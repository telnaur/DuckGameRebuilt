namespace DuckGame.UFFMod
{
    // credits to Garoslaw
    // based off OldPistol

    [EditorGroup("uff|weapons|archery")]
    public class Ballista : Gun
    {
        public StateBinding _loadStateStateBinding = new StateBinding("_loadState");

        public int _loadState; // defines reload state

        private SpriteMap sprite;

        public Ballista(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\ballista"), 32, 23, false);
            graphic = sprite;
            _center = new Vec2(18f, 12f);
            _barrelOffsetTL = new Vec2(32f, 8f);
            _collisionSize = new Vec2(30f, 21f);
            _collisionOffset = new Vec2(-15f, -12f);
            _holdOffset = new Vec2(4, 2);

            // weapon settings
            ammo = 1;
            _ammoType = (AmmoType)new ATArrow(true); // fires ballista arrow instead
            _fireSound = Mod.GetPath<UffMod>("SFX\\ballistaShot");
            _kickForce = 5.0f;
            _weight = 9f;
            flammable = 0.4f;
            physicsMaterial = PhysicsMaterial.Wood;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            if (ammo == 0)
                sprite.frame = 0;
            else
                sprite.frame = 1;

            base.Draw();
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (!(type is DTIncinerate))
                base.OnDestroy(type);
            Level.Remove(this);
            for (int index = 0; index < 8; ++index)
            {
                Thing t = WoodDebris.New(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f));
                t.hSpeed = ((Rando.Float(1f) > 0.5f ? 1f : -1f) * Rando.Float(3f));
                t.vSpeed = -Rando.Float(1f);
                Level.Add(t);
            }
            return true;
        }

        public override void CheckIfHoldObstructed()
        {
            if (duck == null || _loadState == 0)
                return;
            duck.holdObstructed = false;
        }

        public override void OnPressAction()
        {
            if (duck == null)
                return;
            if (ammo > 0)
            {
                Fire();
                ammo = 0;
            }
        }

        public override void Fire()
        {
            SFX.Play(_fireSound, 1f, Rando.Float(0.2f) - 0.1f, 0f, false);
            ApplyKick();

            float bulletAngle = (int)offDir >= 0 ? angleDegrees + _ammoType.barrelAngleDegrees : angleDegrees + 180f - _ammoType.barrelAngleDegrees;
            Bullet bullet = _ammoType.FireBullet(Offset(barrelOffset), owner, bulletAngle, this);
            if (Network.isActive && isServerForObject && duck != null && duck.profile.connection != null)
                bullet.connection = duck.profile.connection;
        }
    }
}