using System;
using DuckGame;

namespace DuckGame.SuperDuck
{
    // The "Eye of Sauron": an autonomous tracker tied to TheOneRing for as long as the ring is
    // WORN. Its single replicated frame counter (_age) is the "hunt clock":
    //
    //   * While the bearer is CLOAKED (ring pushes hunting=true) the clock ADVANCES: the eye wakes
    //     at the top of the level, descends, and closes in.
    //   * While the bearer is UNCLOAKED (hunting=false) the clock RECEDES (counts back down, floored
    //     at 0): the eye retreats toward the top and fades. It is NOT removed — re-cloaking resumes
    //     the hunt from wherever the clock had receded to. Only the ring leaving the bearer
    //     (dropped / removed) resets the hunt, and that is the RING's job (it despawns the eye).
    //
    // Lifecycle phases:
    //   1. DORMANT  (_age < SpawnDelayFrames): exists at the top of the level, invisible, harmless.
    //   2. CHASE    (hunting && awake): fades in, sheds embers, drifts toward the bearer through
    //      walls (no collision — moves its position directly), accelerating SLOWLY.
    //   3. BETRAYAL (chase reaches the bearer): instead of killing, the eye knocks the bearer back
    //      and makes the ring betray them (the ring unequips itself, dropping to the ground). The
    //      eye then enters FINAL PURSUIT.
    //   4. FINAL PURSUIT (betrayed): the eye keeps chasing (the ex-bearer, or the nearest duck) and
    //      KILLS on contact. It self-removes either on that kill or after 3 seconds of the ring
    //      lying un-held by anyone — giving the betrayed bearer a small window to escape.
    //
    // Networking shape follows GandalfFloat / Stun.cs (the proven net-safe pattern in DGR): the
    // target Duck and the eye's own position are StateBound so ghost clients see it move; all the
    // *decisions* (advance/recede, movement, betrayal, kill, despawn) are server-authoritative and
    // gated behind isServerForObject. _age is replicated so the fade/phase/particles stay in
    // lockstep on every client. hunting / betrayed / ringRef are server-only — ghost clients render
    // purely from the replicated _age + position, so they never need them.
    //
    // Cosmetics (sprite glow, ember particles) run on ALL clients un-gated: particles are local,
    // non-networked PhysicsParticles, so each client spawns its own copies — no bandwidth cost.
    //
    // NOTE: string-literal StateBindings, explicit float casts, no nameof/interpolation — mods
    // compile under the C# 5 CodeDom compiler on Windows (docs/modding-guide.md §1.1).
    public class SauronEyeGhost : Thing
    {
        // Server replicates position so ghost clients render the eye where the server has it.
        public StateBinding _positionBinding = new CompressedVec2Binding("position");
        // Without binding the target, ghost copies (whose Duck-taking ctor never runs) have a
        // null target and would bail in Update. Same reasoning as GandalfFloat._duckStateBinding.
        public StateBinding _targetBinding = new StateBinding("targetDuck");
        // Drives the fade-in / phase clock consistently on every client from one server counter.
        public StateBinding _ageBinding = new StateBinding("_age", -1, false, false);
        // Replicated so the kill sting plays on every client (in Removed), not just the server.
        public StateBinding _caughtBinding = new StateBinding("caught");

        public Duck targetDuck;
        public int _age;
        public bool caught;            // set on the final-pursuit kill; drives the kill sting

        // Set by the ring (server-side, co-authoritative with the bearer): true while the bearer is
        // cloaked. Not replicated — only the authority uses it to advance vs. recede the clock.
        public bool hunting;
        // Back-reference to the ring that owns this eye (server-only). Used during final pursuit to
        // tell whether the dropped ring is being held again, which resets the 3s give-up timer.
        public TheOneRing ringRef;

        public bool betrayed;          // server-only: in final-pursuit mode after the betrayal
        private float _finalTimer;     // server-only: frames of "ring un-held" left before giving up
        private float _recoilTimer;    // server-only: frames of post-betrayal recoil left (can't kill yet)
        private float _spoolTimer;     // server-only: frames left to rapidly regain chase speed after recoil

        // Movement is intentionally SLOW: a gentle initial creep and a gentle ramp, so the bearer
        // has a long, readable window to flee or use their cloak before the eye closes in.
        private float _currentSpeed = 0.15f;
        private const float MaxSpeed = 2.2f;
        private const float Acceleration = 0.0032f;     // gentler ramp (was 0.006) — slower to build speed
        private const float CatchRadius = 9f;          // ~a bit larger than a duck half-width
        private const float SpawnDelayFrames = 240f;   // ~4s dormant at the top before it descends
        private const float FadeInFrames = 90f;        // ~1.5s to reach full alpha once awake
        private const float MaxAlpha = 0.95f;
        private const float FinalPursuitFrames = 180f; // ~3s of grace after the betrayal
        private const float RecoilFrames = 45f;        // ~0.75s the eye reels back before the lethal dive
        private const float SpoolFrames = 30f;          // ~0.5s to slam back up to full chase speed after recoil
        private float _glow;
        private bool _chaseStarted;    // local one-shot guard for the chase particles

        // The eye's continuous chase drone (a looped Sound, NOT a one-shot sting). Unlike the cloak
        // loop (local to the wearer) this plays on EVERY client while the eye is actively hunting or
        // in final pursuit, so it layers over the wearer's cloak loop AND warns nearby players the
        // eye is bearing down. Created lazily + defensively so a sound hiccup can't sink the ghost.
        private Sound _chaseLoop;
        private bool _chaseLoopPlaying;

        public SauronEyeGhost(Duck target)
            : base(target != null ? target.x : 0f, SpawnY(target))
        {
            targetDuck = target;
            depth = 0.9f;              // draw above the level

            // Custom art: SuperDuck/content/sprites/saureye.png — a 48px-tall sprite SHEET laid out
            // left-to-right, 48px per frame (so N frames = a 48*N x 48 PNG; e.g. 6 frames = 288x48).
            // We slice it as a SpriteMap and build a looping animation over WHATEVER number of cells
            // the sheet actually has (computed from the texture size below), so a single 48x48 PNG
            // still works (one frame) and a wider sheet animates with no code change. Extension
            // omitted → preloaded texture
            // (docs/modding-guide.md §3.2).
            const int FrameSize = 48;
            SpriteMap sm = new SpriteMap(GetPath("sprites/saureye"), FrameSize, FrameSize);

            // Total cell count = (sheet width / frame) * (sheet height / frame). We read it off the
            // FULL texture: sm.texture.width / sm.texture.height are the whole sheet, whereas
            // sm.width / sm.height are overridden on SpriteMap to the FRAME size. So a single 48x48
            // PNG yields 1 frame and a wider/taller sheet animates with no code change.
            // IMPORTANT: do NOT use sm.frames here — that field is absent on the shipped game
            // assembly the mod compiles against (CodeDom), so referencing it fails to compile.
            int cols = sm.texture.width / FrameSize;
            int rows = sm.texture.height / FrameSize;
            int n = cols * rows;
            if (n < 1)
                n = 1;

            int[] seq = new int[n];
            for (int i = 0; i < n; i++)
                seq[i] = i;
            sm.AddAnimation("glare", 0.2f, true, seq);   // speed = frames advanced per tick
            sm.SetAnimation("glare");
            graphic = sm;
            center = new Vec2(FrameSize / 2f, FrameSize / 2f);
            alpha = 0f;                // dormant: invisible until it wakes
        }

        // The eye is born at the very top of the level so it can descend onto the bearer.
        // Static so it can be used in the base() initializer; falls back to "above the duck"
        // if the level bounds aren't available for some reason.
        private static float SpawnY(Duck target)
        {
            if (Level.current != null)
                return Level.current.topLeft.y + 8f;
            return target != null ? target.y - 160f : 0f;
        }

        // The eye's "home" altitude at the top of the level (where it lurks while dormant/receding).
        private float TopY()
        {
            if (Level.current != null)
                return Level.current.topLeft.y + 8f;
            return position.y;
        }

        public override void Update()
        {
            base.Update();

            if (targetDuck == null)
            {
                // Ghost client whose targetDuck binding hasn't arrived yet — wait for it.
                // If WE are the authority and it's still null, something is wrong: bail.
                if (isServerForObject)
                    Level.Remove(this);
                return;
            }

            _glow += 0.2f;

            // ── FINAL PURSUIT ───────────────────────────────────────────────────────
            // After the betrayal the eye is manifest and angry: full alpha, a heavy ember trail,
            // and it hunts to KILL. It outlives the ring leaving the bearer; the ring no longer
            // owns it (handed off in TheOneRing.BeginBetrayal), so it self-manages its own end.
            if (betrayed)
            {
                alpha = MaxAlpha;
                StartChaseLoop();   // the drone keeps screaming through the lethal final pursuit
                SpawnTrail(2.5f);   // a heavy, angry ember/smoke trail in final pursuit
                if (isServerForObject)
                    UpdateFinalPursuit();
                return;
            }

            // Outside of final pursuit, if the bearer is gone the hunt is over.
            if (targetDuck.dead || targetDuck.removeFromLevel)
            {
                if (isServerForObject)
                    Level.Remove(this);
                return;
            }

            // Advance the hunt clock while cloaked, recede it while uncloaked (authority only;
            // clients receive _age via its binding). Request: uncloaking DECREASES the timer
            // instead of resetting it, so repeated cloak/uncloak still accumulates danger.
            if (isServerForObject)
            {
                if (hunting)
                    _age++;
                else
                    _age = Math.Max(_age - 1, 0);
            }

            bool awake = _age >= SpawnDelayFrames;

            if (hunting && awake)
            {
                // CHASE. First awake frame on THIS client: a one-shot ember puff to punctuate the
                // descent. The chase SOUND is now a continuous loop (below), not a one-shot sting.
                if (!_chaseStarted)
                {
                    _chaseStarted = true;
                    for (int i = 0; i < 8; i++)
                        SpawnGlowEmber(2.5f);
                }
                StartChaseLoop();   // continuous chase drone for as long as the eye is hunting

                float activeAge = _age - SpawnDelayFrames;
                alpha = Math.Min(activeAge / FadeInFrames, MaxAlpha);
                SpawnTrail(1.2f);   // a steady ember/smoke wake while hunting

                // Movement + betrayal are server-authoritative; clients render the synced position.
                if (isServerForObject)
                {
                    Vec2 dir = targetDuck.position - position;
                    float len = dir.length;
                    if (len > 0.01f)
                    {
                        dir = dir * (1f / len);
                        _currentSpeed = Math.Min(_currentSpeed + Acceleration, MaxSpeed);
                        position += dir * _currentSpeed;
                    }

                    // Grace period: the eye cannot reach the bearer until it has fully faded in, so
                    // the player always gets the visible "something is hunting" tell first.
                    bool fadedIn = activeAge >= FadeInFrames;

                    // Anyone OTHER than the bearer the eye passes through is killed outright (it's a
                    // lethal manifestation, not just the bearer's problem). Only lethal once awake +
                    // faded in, same grace rule as the betrayal.
                    if (fadedIn)
                        KillTouchedDucks(targetDuck);

                    // Reaching the bearer triggers the betrayal (knockback + ring drop), not a kill.
                    if (fadedIn && !removeFromLevel && Collision.Circle(position, CatchRadius, targetDuck))
                        Betray();
                }
            }
            else
            {
                // DORMANT (not yet awake) or RECEDING (uncloaked while still awake): retreat to the
                // top of the level above the bearer's column and fade out as the clock winds down.
                StopChaseLoop();   // not actively hunting → silence the drone
                float activeAge = _age - SpawnDelayFrames;
                alpha = awake ? Math.Min(activeAge / FadeInFrames, MaxAlpha) : 0f;

                if (isServerForObject)
                {
                    position.x = Lerp.Float(position.x, targetDuck.x, 0.08f);
                    position.y = Lerp.Float(position.y, TopY(), 0.1f);
                    _currentSpeed = 0.15f;
                }

                // Once it has fully receded to dormant, re-arm the chase sting so a fresh re-cloak
                // re-triggers the descent dramatically.
                if (!awake)
                    _chaseStarted = false;
            }
        }

        // ── Betrayal: reach the worn bearer → knock them back, make the ring betray them ─────────
        private void Betray()
        {
            betrayed = true;
            _finalTimer = FinalPursuitFrames;
            _recoilTimer = RecoilFrames;   // reel back first — gives the betrayed duck separation to flee
            _currentSpeed = 0.15f;         // and reset the speed ramp so the resumed dive starts slow

            // Dark-energy burst + sting at the moment of betrayal.
            for (int i = 0; i < 3; i++)
                Level.Add(new ExplosionPart(targetDuck.x + Rando.Float(-4f, 4f), targetDuck.y + Rando.Float(-4f, 4f)));
            SFX.Play(GetPath("SFX/onering_catch"), 0.9f);

            // The ring does the knockback + drops itself off the bearer (co-authoritative with the
            // duck) and hands this eye off to autonomous final pursuit.
            if (ringRef != null && !ringRef.removeFromLevel)
                ringRef.BeginBetrayal(position);
        }

        // ── Final pursuit: chase to KILL, give up after 3s of the ring lying un-held ─────────────
        private void UpdateFinalPursuit()
        {
            // The 3-second clock only drains while the ring is NOT held by anyone. If someone grabs
            // the dropped ring (or the ring is gone entirely we treat as un-held), the eye keeps
            // bearing down. Run out the clock → the eye gives up and fades away.
            bool ringHeld = ringRef != null && !ringRef.removeFromLevel && ringRef.IsHeldOrWorn();
            if (ringHeld)
                _finalTimer = FinalPursuitFrames;
            else
                _finalTimer -= 1f;

            if (_finalTimer <= 0f)
            {
                Level.Remove(this);   // gave the bearer their window; vanish
                return;
            }

            // Pursue the ex-bearer if they're still around, else the nearest duck.
            Duck prey = targetDuck;
            if (prey == null || prey.dead || prey.removeFromLevel)
                prey = NearestDuck();
            if (prey == null)
                return;

            // RECOIL: the instant it betrays, the eye is already INSIDE CatchRadius — without a
            // recoil it would re-collide and kill on the very next frame, so the betrayal "zap +
            // knockback + drop" would never be seen. For ~0.75s the eye reels back up toward the
            // top of the level (drifting only slowly toward the prey's column) and CANNOT kill,
            // giving the betrayed duck real separation before the lethal dive resumes.
            if (_recoilTimer > 0f)
            {
                _recoilTimer -= 1f;
                position.y = Lerp.Float(position.y, TopY(), 0.18f);
                position.x = Lerp.Float(position.x, prey.x, 0.04f);
                _currentSpeed = 0.15f;
                _spoolTimer = SpoolFrames;   // arm the rapid speed-up for the instant recoil ends
                return;
            }

            float pursuitMax = MaxSpeed * 1.3f;
            // After the recoil the eye is committed and must NOT crawl back up to speed slowly —
            // it slams back to full chase speed within ~0.5s (SpoolFrames). We lerp hard toward the
            // pursuit max during the spool window, then hold at the cap.
            if (_spoolTimer > 0f)
            {
                _spoolTimer -= 1f;
                _currentSpeed = Lerp.Float(_currentSpeed, pursuitMax, 0.16f);
            }
            else
            {
                _currentSpeed = Math.Min(_currentSpeed + Acceleration * 1.5f, pursuitMax);
            }

            Vec2 dir = prey.position - position;
            float len = dir.length;
            if (len > 0.01f)
            {
                dir = dir * (1f / len);
                position += dir * _currentSpeed;
            }

            // Final pursuit is lethal to ANY duck it reaches, not just the ex-bearer. If it claims
            // anyone, its work is done — flash the kill and vanish.
            if (KillTouchedDucks(null))
            {
                caught = true;   // replicated → the kill sting plays on every client in Removed()
                Level.Remove(this);
            }
        }

        // Kills every duck currently within CatchRadius except `spare` (pass null to spare nobody).
        // Returns true if it killed at least one. Server-authoritative — callers are already gated.
        private bool KillTouchedDucks(Duck spare)
        {
            bool any = false;
            foreach (Duck dk in Level.current.things[typeof(Duck)])
            {
                if (dk == null || dk.dead || dk.removeFromLevel || dk == spare)
                    continue;
                if (Collision.Circle(position, CatchRadius, dk))
                {
                    KillDuck(dk);
                    any = true;
                }
            }
            return any;
        }

        private void KillDuck(Duck d)
        {
            for (int i = 0; i < 4; i++)
                Level.Add(new ExplosionPart(d.x + Rando.Float(-5f, 5f), d.y + Rando.Float(-5f, 5f)));
            d.Kill(new DTImpact(this));
        }

        private Duck NearestDuck()
        {
            Duck best = null;
            float bestLen = float.MaxValue;
            foreach (Duck dk in Level.current.things[typeof(Duck)])
            {
                if (dk == null || dk.dead || dk.removeFromLevel)
                    continue;
                float l = (dk.position - position).length;
                if (l < bestLen)
                {
                    bestLen = l;
                    best = dk;
                }
            }
            return best;
        }

        // A small glowing ember at the eye. Embers are local, non-networked PhysicsParticles, so
        // spawning them in Update on every client is safe and free of network traffic.
        private void SpawnGlowEmber(float spread)
        {
            Ember e = new Ember(position.x + Rando.Float(-spread, spread), position.y + Rando.Float(-spread, spread));
            Level.Add(e);
        }

        // A richer per-frame particle wake: several embers plus the occasional wisp of smoke, all
        // local/non-networked so every client renders its own (no bandwidth). `intensity` scales how
        // many embers fly off — the hunt is a steady simmer, final pursuit is a furnace.
        private void SpawnTrail(float intensity)
        {
            int embers = 1 + (int)intensity;
            for (int i = 0; i < embers; i++)
                SpawnGlowEmber(2f * intensity);

            // A drifting ash/smoke puff so the trail reads as smoke-and-fire, not just sparks.
            if (Rando.Float(1f) < 0.25f + 0.15f * intensity)
                SmallSmoke.New(position.x + Rando.Float(-3f, 3f), position.y + Rando.Float(-3f, 3f));

            // Hot inner flecks during the heavier (final-pursuit) trail.
            if (intensity > 1.5f && _age % 2 == 0)
                Level.Add(new ExplosionPart(position.x + Rando.Float(-4f, 4f), position.y + Rando.Float(-4f, 4f)));
        }

        // ── Chase drone (looped, on every client while hunting/pursuing) ──────────────
        private void StartChaseLoop()
        {
            try
            {
                if (_chaseLoop == null)
                    _chaseLoop = SFX.Get(GetPath("SFX/onering_chase"), 0.85f, looped: true);
                if (_chaseLoop != null && !_chaseLoopPlaying)
                {
                    _chaseLoop.Volume = 0.85f;
                    _chaseLoop.Play();
                    _chaseLoopPlaying = true;
                }
            }
            catch
            {
                _chaseLoop = null;
            }
        }

        private void StopChaseLoop()
        {
            if (_chaseLoop != null && _chaseLoopPlaying)
                _chaseLoop.Stop();
            _chaseLoopPlaying = false;
        }

        public override void Removed()
        {
            StopChaseLoop();   // never leave the drone playing after the eye is gone
            // Runs on every client (server self-remove AND network ghost removal). Play the kill
            // sting where the eye died only when it actually claimed a duck — `caught` is StateBound
            // so ghost clients see the true value here.
            if (caught)
                SFX.Play(GetPath("SFX/onering_catch"), 0.9f);
            base.Removed();
        }

        public override void Draw()
        {
            // Sprite (drawn by base.Draw via `graphic`) plus an additive glow halo so the eye
            // reads as a hot, pulsing light. Skipped entirely while dormant (alpha 0).
            if (alpha > 0.01f)
            {
                float pulse = 0.7f + 0.3f * (float)Math.Sin(_glow);
                float r = 6f + pulse * 2f;
                Color halo = new Color(255, 80, 0) * (alpha * 0.35f * pulse);
                int segs = 12;
                for (int k = 0; k < segs; k++)
                {
                    float a0 = (float)(k * 2.0 * Math.PI / segs);
                    float a1 = (float)((k + 1) * 2.0 * Math.PI / segs);
                    Vec2 p0 = position + new Vec2((float)Math.Cos(a0) * r, (float)Math.Sin(a0) * r);
                    Vec2 p1 = position + new Vec2((float)Math.Cos(a1) * r, (float)Math.Sin(a1) * r);
                    Graphics.DrawLine(p0, p1, halo, 2f, depth - 1);
                }
            }

            base.Draw();
        }
    }
}
