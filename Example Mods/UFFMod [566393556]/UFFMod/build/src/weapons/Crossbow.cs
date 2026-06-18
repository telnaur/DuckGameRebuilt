namespace DuckGame.UFFMod
{
    // based off decompiled Old Pistol

    [EditorGroup("uff|weapons|archery")]
    public class Crossbow : Gun
    {
        public StateBinding _loadStateStateBinding = new StateBinding("_loadState");

        public int _loadState;

        private float angleOffset;

        private SpriteMap sprite;

        public Crossbow(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\crossbow"), 36, 32);
            graphic = sprite;
            _center = new Vec2(16f, 16f);
            _barrelOffsetTL = new Vec2(32f, 12f);
            _collisionSize = new Vec2(24f, 10f);
            _collisionOffset = new Vec2(-8f, -8f);

            // weapon settings
            ammo = 99;
            _ammoType = new ATArrow();
            _ammoType.bulletSpeed = 24f;
            _fireSound = Mod.GetPath<UffMod>("SFX\\bowShotSFX");
            _kickForce = 0.2f;
            _weight = 7.25f;
            flammable = 0.4f;
            physicsMaterial = PhysicsMaterial.Wood;

            // defaults
            _loadState = 0;
        }

        public override void Update()
        {
            if (_loadState == -1 || _loadState >= 1)
                _hasTrigger = false;
            else
                _hasTrigger = true;
            if (_loadState == 1)
            {
                if (angleOffset > -0.7)
                    angleOffset = MathHelper.Lerp(angleOffset, -0.8f, 0.12f);
                else
                    _loadState++;
            }
            else if (_loadState == 2)
            {
                if (handOffset.x < 2)
                {
                    handOffset.x += 0.08f;
                    handOffset.y -= 0.24f;
                }
                else
                    _loadState++;
            }
            else if (_loadState >= 3 && _loadState != 5)
            {
                if (handOffset.x > 0)
                {
                    handOffset.x -= 0.08f;
                    handOffset.y += 0.24f;
                }
                else
                    _loadState++;
                if (angleOffset < 0)
                    angleOffset = MathHelper.Lerp(angleOffset, 0f, 0.24f);
                else
                    _loadState++;
            }
            else if (_loadState == 5)
                _loadState = 0;
            base.Update();
        }

        public override void Draw()
        {
            if (_loadState == 0 || _loadState >= 3)
                sprite.frame = 1;
            else
                sprite.frame = 0;

            float angle = this.angle;
            if ((int)this.offDir > 0)
                this.angle = this.angle - this.angleOffset;
            else
                this.angle = this.angle + this.angleOffset;
            base.Draw();
            this.angle = angle;
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
            if (duck != null && _loadState != 0)
                duck.holdObstructed = false;
        }

        public override void OnPressAction()
        {
            if (duck == null)
                return;
            if (_loadState == 0)
            {
                base.OnPressAction();
                _loadState = -1;
            }
            else if (_loadState == -1)
            {
                _loadState = 1;
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
