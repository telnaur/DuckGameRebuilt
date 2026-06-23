using DuckGame;

namespace DuckGame.SuperDuck
{
    // A short-lived status effect, one per affected duck, spawned by GandalfsStaff. It puts
    // the target into a ragdoll and gently lifts it for a few seconds, then lets it drop and
    // recover. Pattern (constructor-takes-Duck, StateBindings for the target + timer,
    // server-authoritative physics nudges) follows the Ostrich mod's Stun.cs, which is the
    // proven network-safe shape for this in DGR.
    public class GandalfFloat : Thing
    {
        // ~60 fps, so 180 frames is the requested 3 seconds of floating.
        private const int DurationFrames = 180;
        // Target upward drift while weightless (negative Y is up). Small = "slowly".
        private const float FloatSpeed = -0.5f;

        private Duck duck;
        private Ragdoll Rag;
        private int f;

        // NOTE: string literals, not nameof() — DGR compiles mods at runtime with the legacy
        // CodeDom C# compiler (C# 5 max), so nameof/string-interpolation/etc. won't compile.
        public StateBinding _frames = new StateBinding("f", -1, false, false);
        // Without this, ghost copies on remote clients have a null duck (the Duck-taking
        // constructor never runs for ghosts) and remove themselves in Initialize(). See Stun.cs.
        public StateBinding _duckStateBinding = new StateBinding("duck");

        public GandalfFloat(Duck duck)
            : base(0f)
        {
            this.duck = duck;
        }

        public override void Initialize()
        {
            // On ghost clients, StateBindings (including duck) are not populated until after
            // Initialize() returns. If duck is null here we fall through; Update() will start
            // the float on the first frame the StateBinding arrives. Never call Level.Remove
            // here for null duck — that silently kills the ghost before it can do anything.
            if (duck != null)
            {
                duck.GoRagdoll();
                Rag = duck.ragdoll;
                MakeWeightless();
            }
            base.Initialize();
        }

        public override void Update()
        {
            if (duck == null)
            {
                // Ghost client: duck StateBinding hasn't arrived yet. The server always has
                // duck set (constructor ran), so if we're the authority and duck is null
                // something genuinely went wrong — bail out.
                if (isServerForObject)
                    Level.Remove(this);
                return;
            }

            if (duck.dead)
            {
                Level.Remove(this);
                return;
            }

            // Keep the duck ragdolled for the duration. GoRagdoll() is a no-op if already
            // ragdolled, and clearing _makeActive stops the duck from standing back up early.
            // Also handles the ghost-path first-frame where Initialize() saw null duck.
            duck.GoRagdoll();
            if (Rag == null)
                Rag = duck.ragdoll;
            if (Rag != null)
                Rag._makeActive = false;

            // Physics is authoritative on whoever owns the duck; the rest receive the synced
            // ragdoll positions, so only nudge there.
            if (duck.isServerForObject)
                ApplyFloat();

            f++;
            if (f >= DurationFrames)
                Level.Remove(this);

            base.Update();
        }

        public override void Removed()
        {
            // Runs on ALL clients (including when the network manager removes the ghost),
            // so this is the only reliable place to undo the float effect everywhere.
            Restore();
            base.Removed();
        }

        // Cancel gravity on the ragdoll parts so they hang weightless instead of falling.
        // extraGravMultiplier is the right knob: RagdollPart.currentGravity multiplies by it,
        // and (unlike gravMultiplier) it is NOT reset every frame by RagdollPart.Update.
        private void MakeWeightless()
        {
            if (Rag == null)
                return;
            if (Rag.part1 != null) Rag.part1.extraGravMultiplier = 0f;
            if (Rag.part2 != null) Rag.part2.extraGravMultiplier = 0f;
            if (Rag.part3 != null) Rag.part3.extraGravMultiplier = 0f;
        }

        private void ApplyFloat()
        {
            if (Rag == null)
                return;
            MakeWeightless();
            Lift(Rag.part1);
            Lift(Rag.part2);
            Lift(Rag.part3);
        }

        private static void Lift(RagdollPart part)
        {
            if (part == null || part.owner != null) // don't fight a duck that grabbed the part
                return;
            // Ease toward a slow upward drift and bleed off sideways motion so it rises
            // straight-ish rather than flinging across the level.
            part.vSpeed = Lerp.Float(part.vSpeed, FloatSpeed, 0.08f);
            part.hSpeed *= 0.94f;
        }

        // Give weight back and let the duck recover (drop, then stand up). Unragdoll handles
        // immobilized/physics; we flip _makeActive so the duck's own Update unragdolls it.
        private void Restore()
        {
            if (Rag != null)
            {
                if (Rag.part1 != null) Rag.part1.extraGravMultiplier = 1f;
                if (Rag.part2 != null) Rag.part2.extraGravMultiplier = 1f;
                if (Rag.part3 != null) Rag.part3.extraGravMultiplier = 1f;
                Rag._makeActive = true;
            }
            if (duck != null)
                duck.immobilized = false;
        }
    }
}
