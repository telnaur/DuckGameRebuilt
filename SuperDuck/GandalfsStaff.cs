using System;
using DuckGame;

namespace DuckGame.SuperDuck
{
    // A staff that "casts" rather than shoots: on press it lifts every OTHER duck into a
    // slow, floating ragdoll for a few seconds. Modeled on Gun so it benefits from all the
    // built-in hold/throw/ammo/network plumbing; Fire() is overridden to do the spell
    // instead of spawning a bullet.
    // NOTE: aligned to the shape proven by working mod guns (Ostrich's Mortix/ElectricStaff,
    // Weaponized's wizardStaff): plain fields only — no [BaggedProperty] attributes — plus the
    // standard weapon setup (_editorName, _type, explicit sprite dimensions, GetPath idiom).
    [EditorGroup("SuperDuck")]
    public class GandalfsStaff : Gun
    {
        // Plays a small "raise the staff" arm animation when cast; synced so remote clients
        // see the gesture. String literal, not nameof() — mods compile under the C# 5 CodeDom
        // compiler (docs/modding-guide.md §1.1).
        public StateBinding _raiseArmBinding = new StateBinding("_raiseArm");
        public float _raiseArm;

        public GandalfsStaff(float xval, float yval)
            : base(xval, yval)
        {
            _editorName = "Gandalf's Staff";
            _type = "gun";                  // standard weapon category (every vanilla gun sets this)
            ammo = 12;
            // We never actually fire a bullet (Fire() is fully overridden), but Gun's fire path
            // (Reload / ammo display) expects a non-null AmmoType. A cheap reused type is fine.
            _ammoType = new AT9mm
            {
                range = 200f
            };
            isFatal = false;                // staff itself deals no damage (real Gun field, not a bag)

            // Custom art: SuperDuck/content/sprites/gandalfsstaff.png (11x48, single frame).
            // Use the instance GetPath idiom (extension omitted → preloaded texture,
            // docs/modding-guide.md §3.2). NOTE: the trailing ints in the examples'
            // `new Sprite(path, 13, 29)` are the Sprite(string,float x,float y) POSITION args
            // (Sprite.cs:111), not frame size — and Thing.graphic position is overwritten each
            // draw, so they're a no-op. A single-frame sprite takes just the path; a multi-frame
            // sheet would use `new SpriteMap(path, frameW, frameH)` instead.
            graphic = new Sprite(GetPath("sprites/gandalfsstaff"));
            // Grip near the lower portion of the shaft so the duck holds the staff upright;
            // tune these to taste against the art.
            center = new Vec2(3f, 20f);
            collisionOffset = new Vec2(-3f, -20f);
            collisionSize = new Vec2(6f, 28f);
            _barrelOffsetTL = new Vec2(3f, 0f);   // tip of the staff (top of the sprite)
            _holdOffset = new Vec2(2f, 0f);

            // Custom cast SFX: SuperDuck/content/SFX/magicstaff.wav. Mod sounds register under
            // their full GetPath key (path minus extension), so reference it the same way.
            _fireSound = GetPath("SFX/magicstaff");
            _fullAuto = false;              // press-to-cast
            _fireWait = 12f;                // long cooldown between casts
            _kickForce = 0f;
            editorTooltip = "YOU SHALL NOT... stay on the ground. Lifts every other duck into a slow floating ragdoll.";
        }

        public override void Fire()
        {
            // Mirror Gun's own gates so the cast respects ammo / cooldown / reload state.
            if (!loaded || ammo <= 0 || _wait > 0f)
                return;

            CastLevitation();

            _raiseArm = 1f;
            Reload(false);                  // decrement ammo, no shell casing
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
            handAngle = _raiseArm * 1.2f * offDir;
        }
    }
}
