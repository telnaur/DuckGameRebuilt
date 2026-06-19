using System;
using DuckGame;

namespace DuckGame.SuperDuck
{
    // A staff that "casts" rather than shoots: on press it lifts every OTHER duck into a
    // slow, floating ragdoll for a few seconds. Modeled on Gun so it benefits from all the
    // built-in hold/throw/ammo/network plumbing; Fire() is overridden to do the spell
    // instead of spawning a bullet.
    [EditorGroup("SuperDuck")]
    [BaggedProperty("isFatal", false)]
    [BaggedProperty("isSuperWeapon", true)]
    public class GandalfsStaff : Gun
    {
        // Plays a small "raise the staff" arm animation when cast; synced so remote clients
        // see the gesture.
        public StateBinding _raiseArmBinding = new StateBinding(nameof(_raiseArm));
        public float _raiseArm;

        public GandalfsStaff(float xval, float yval)
            : base(xval, yval)
        {
            ammo = 3;
            // We never actually fire a bullet (Fire() is fully overridden), but Gun expects a
            // non-null AmmoType for ammo display / range. A cheap reused type is fine.
            _ammoType = new AT9mm
            {
                range = 200f
            };

            // Custom art: SuperDuck/content/sprites/gandalfsstaff.png (11x48, a tall vertical
            // staff). Referenced by GetPath without the .png extension so it resolves to the
            // preloaded texture (see docs/modding-guide.md §3.2).
            graphic = new Sprite(Mod.GetPath<SuperDuckMod>("sprites/gandalfsstaff"));
            // Grip near the lower portion of the shaft so the duck holds the staff upright;
            // tune these to taste against the art.
            center = new Vec2(5f, 38f);
            collisionOffset = new Vec2(-3f, -24f);
            collisionSize = new Vec2(6f, 48f);
            _barrelOffsetTL = new Vec2(5f, 0f);   // tip of the staff (top of the sprite)

            // Custom cast SFX: SuperDuck/content/SFX/magicstaff.wav. Mod sounds are registered
            // under their full GetPath key (path minus extension), so reference it the same way.
            _fireSound = Mod.GetPath<SuperDuckMod>("SFX/magicstaff");
            _fullAuto = false;              // press-to-cast
            _fireWait = 12f;                // long cooldown between casts
            _kickForce = 0f;
            _holdOffset = new Vec2(2f, 0f);
            editorTooltip = "YOU SHALL NOT... stay on the ground. Lifts every other duck into a slow floating ragdoll.";
        }

        public override void Fire()
        {
            // Mirror Gun's own gates so the cast respects ammo / cooldown / reload state.
            if (!loaded || ammo <= 0 || _wait > 0f)
                return;

            CastLevitation();

            _raiseArm = 1f;
            Reload();                       // decrement ammo + pop the "shot"
            _wait = _fireWait;
            PlayFireSound();
        }

        private void CastLevitation()
        {
            // Authority rule: only the staff's owner spawns the networked effect Things.
            // If every client spawned them we'd flood the level with ghosts and desync/crash
            // online (this exact bug bit the Ostrich mod — see ToyHammer.cs).
            if (Network.isActive && !isServerForObject)
                return;

            Duck caster = duck;
            foreach (Duck target in Level.current.things[typeof(Duck)])
            {
                if (target == null || target.dead || target == caster)
                    continue;

                Level.Add(new GandalfFloat(target));
            }
        }

        public override void Update()
        {
            base.Update();
            // Ease the raised arm back down after a cast.
            if (_raiseArm > 0f)
                _raiseArm = Lerp.Float(_raiseArm, 0f, 0.1f);
            handAngle = _raiseArm * -1.2f * offDir;
        }
    }
}
