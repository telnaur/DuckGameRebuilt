namespace DuckGame.UFFMod
{
    // based off decompiled Grenade Launcher

    [EditorGroup("uff|weapons|archery")]
    public class Bow : Gun
    {
        public StateBinding _fireAngleState = new StateBinding("_fireAngle");
        public StateBinding _aimAngleState = new StateBinding("_aimAngle");
        public StateBinding _aimWaitState = new StateBinding("_aimWait");
        public StateBinding _aimingState = new StateBinding("_aiming");
        public StateBinding _firingState = new StateBinding("_firing");
        public StateBinding _cooldownState = new StateBinding("_cooldown");
        public float _fireAngle;
        public float _aimAngle;
        public float _aimWait;
        public bool _aiming;
        public bool _firing;
        public float _cooldown;

        public override float angle
        {
            get
            {
                return base.angle + _aimAngle;
            }
            set
            {
                _angle = value;
            }
        }

        public Bow(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // collision & sprite settings
            graphic = new SpriteMap(Mod.GetPath<UffMod>("weapons\\bow"), 24, 26);
            _center = new Vec2(12f, 13f);
            _barrelOffsetTL = new Vec2(15f, 13f);
            _collisionSize = new Vec2(10f, 20f);
            _collisionOffset = new Vec2(-5f, -10f);

            // weapon settings
            ammo = 6;
            _ammoType = new ATArrow();
            _fireSound = Mod.GetPath<UffMod>("SFX\\bowShotSFX.wav");
            _kickForce = 0.2f;
            flammable = 0.4f;
            _hasTrigger = false;
            physicsMaterial = PhysicsMaterial.Wood;
        }

        public override void Draw()
        {
            if (_aiming)
                frame = 1;
            else
                frame = 0;
           base.Draw();
        }

        public override void Update()
        {
            base.Update();
            if (_aiming && _aimWait <= 0f && _fireAngle < 80f)
            {
                _ammoType.bulletSpeed += 0.14f;
                _fireAngle += 1.6f;
            }
            if (_aimWait > 0.0)
                _aimWait -= 0.9f;
            if (_cooldown > 0.0)
                _cooldown -= 0.1f;
            else
                _cooldown = 0f;
            if (owner != null)
            {
                _aimAngle = -Maths.DegToRad(_fireAngle);
                if (offDir < 0)
                    _aimAngle = -_aimAngle;
            }
            else
            {
                _aimWait = 0f;
                _aiming = false;
                _aimAngle = 0f;
                _fireAngle = 0f;
            }
            if (!_raised)
                return;
            _aimAngle = 0.0f;
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

        public override void OnPressAction()
        {
            if (owner == null)
                return;
            if (_cooldown != 0f)
                return;
            if (ammo > 0)
            {
                _aiming = true;
                _aimWait = 1f;
                _ammoType.bulletSpeed = 6f;
            }
        }

        public override void OnReleaseAction()
        {
            if (_cooldown != 0 || ammo <= 0)
                return;
            _aiming = false;
            Fire();
            _cooldown = 1f;
            angle = 0f;
            _fireAngle = 0f;
        }

        public override void Fire()
        {
            if (!loaded)
                return;
            firedBullets.Clear();
            if (ammo > 0 && _wait == 0)
            {
                ApplyKick();
                for (int index = 0; index < _numBulletsPerFire; ++index)
                {
                    float num = _ammoType.accuracy;
                    _ammoType.accuracy *= 1f - _accuracyLost;
                    _ammoType.bulletColor = _bulletColor;
                    float angle = offDir >= 0 ? angleDegrees + _ammoType.barrelAngleDegrees : angleDegrees + 180f - _ammoType.barrelAngleDegrees;
                    if (!receivingPress)
                    {
                        Bullet bullet = _ammoType.FireBullet(Offset(barrelOffset), owner, angle, this);
                        if (Network.isActive && isServerForObject)
                        {
                            firedBullets.Add(bullet);
                            if (duck != null && duck.profile.connection != null)
                                bullet.connection = duck.profile.connection;
                        }
                    }
                    ++bulletFireIndex;
                    _ammoType.accuracy = num;
                }
                loaded = false;
                if (!_manualLoad)
                    Reload(true);
                firing = true;
                _wait = _fireWait;
                PlayFireSound();
                if (owner == null)
                {
                    Vec2 vec2 = barrelVector * Rando.Float(1f, 3f);
                    vec2.y += Rando.Float(2f);
                    hSpeed -=vec2.x;
                    vSpeed -= vec2.y;
                }
                _accuracyLost += loseAccuracy;
                if (_accuracyLost <= maxAccuracyLost)
                    return;
                _accuracyLost = maxAccuracyLost;
            }
            else
            {
                if (ammo > 0 || _wait != 0f)
                    return;
                _wait = _fireWait;
            }
        }
    }
}