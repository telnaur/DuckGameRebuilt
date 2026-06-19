using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|stuff|hazards")]
    [BaggedProperty("canSpawn", false)]
    public class SentryTurret : Holdable
    {
        public StateBinding _targetStateBinding = new StateBinding("_target");
        public StateBinding _flareAlphaStateBinding = new StateBinding("_flareAlpha");
        public StateBinding _gunAngleStateBinding = new StateBinding("_gunAngle");
        public StateBinding _currRampStateBinding = new StateBinding("_cooldown");
        public StateBinding _shotsFiredStateBinding = new StateBinding("_shotsFired");
        public StateBinding _rampedStateBinding = new StateBinding("_ramped");
        public StateBinding _playedBeepStateBinding = new StateBinding("_playedBeep");
        public StateBinding _laserSightStateBinding = new StateBinding("_laserSight");
        public StateBinding _laserInitStateBinding = new StateBinding("_laserInit");
        public StateBinding _wallPointStateBinding = new CompressedVec2Binding("_wallPoint");
        public StateBinding netSFX_beepStateBinding = new NetSoundBinding("netSFX_beep");
        public StateBinding netSFX_bulletStateBinding = new NetSoundBinding("netSFX_bullet");
        public StateBinding netSFX_laserStateBinding = new NetSoundBinding("netSFX_laser");
        public StateBinding netSFX_missileStateBinding = new NetSoundBinding("netSFX_missile");

        public NetSoundEffect netSFX_beep = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\beepbeep")
        });
        public NetSoundEffect netSFX_bullet = new NetSoundEffect(new string[1]
        {
            "pistolFire"
        });
        public NetSoundEffect netSFX_laser = new NetSoundEffect(new string[1]
        {
            "laserRifle"
        });
        public NetSoundEffect netSFX_missile = new NetSoundEffect(new string[1]
        {
            "missile"
        });

        public PhysicsObject _target;
        public float _flareAlpha;
        public float _gunAngle;
        public float _cooldown;
        public int _shotsFired;
        public bool _ramped;
        public bool _playedBeep;
        public bool _laserSight;
        public bool _laserInit;
        public Vec2 _wallPoint;

        public EditorProperty<int> shotType = new EditorProperty<int>(0, null, 0f, 2f, 1f);
        public EditorProperty<int> shots = new EditorProperty<int>(30, null, 1f, 99f, 1f);
        public EditorProperty<float> delay = new EditorProperty<float>(0.1f, null, 0f, 3f, 0.05f);
        public EditorProperty<float> rampup = new EditorProperty<float>(0.5f, null, 0f, 3f, 0.05f);

        public List<Bullet> firedBullets = new List<Bullet>();

        private AmmoType ammoType;
        private SpriteMap bulletSprite;
        private SpriteMap laserSprite;
        private SpriteMap rocketSprite;
        private SpriteMap gunSprite;
        private SpriteMap standSprite;
        private SpriteMap flare;
        private Sprite _sightHit;
        private Tex2D _laserTex;

        private Vec2 firePosition
        {
            get
            {
                return position + new Vec2(offDir * 5f, -6f - (shotType == 0 ? 3f : (shotType == 1 ? 2f : 0f))) + (17f * new Vec2(offDir * 1f, 0f).Rotate(_gunAngle, Vec2.Zero));
            }
        }

        public SentryTurret(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Sentry Turret";

            // general settings
            bulletSprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\hazards\\turretGun"), 28, 9);
            bulletSprite.AddAnimation("idle", 1f, false, 0);
            bulletSprite.AddAnimation("firing", 0.5f, true, 0, 1, 2);
            laserSprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\hazards\\turretGunLaser"), 28, 9);
            laserSprite.AddAnimation("idle", 1f, false, 0);
            laserSprite.AddAnimation("firing", 1f, true, 0);
            rocketSprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\hazards\\turretGunRocket"), 28, 9);
            rocketSprite.AddAnimation("idle", 1f, false, 0);
            rocketSprite.AddAnimation("firing", 1f, true, 0);
            standSprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\hazards\\turretStand"), 16, 13);
            graphic = standSprite;
            flare = new SpriteMap("smallFlare", 11, 10);
            flare.center = new Vec2(0f, 5f);
            bulletSprite.center = new Vec2(9f, 5f);
            laserSprite.center = new Vec2(9f, 5f);
            rocketSprite.center = new Vec2(9f, 5f);
            center = new Vec2(8f, 6f);
            collisionOffset = new Vec2(-8f, -6f);
            collisionSize = new Vec2(16f, 13f);
            _holdOffset = new Vec2(-6f, 4f);
            depth = 0.5f;
            thickness = 0.3f;
            weight = 10f;
            flammable = 0f;
            physicsMaterial = PhysicsMaterial.Metal;
            _gunAngle = angle;
            ammoType = new AT9mm();

            gunSprite = bulletSprite;
            _sightHit = new Sprite("laserSightHit");
            _sightHit.CenterOrigin();
            _laserTex = Content.Load<Tex2D>("pointerLaser");

            shotType = new EditorProperty<int>(0, this, 0f, 2f, 1f); // re-initialising for notify
        }

        public override void EditorPropertyChanged(object property)
        {
            ChangeAmmo();

            base.EditorPropertyChanged(property);
        }

        private void ChangeAmmo()
        {
            switch (shotType)
            {
                default:
                    gunSprite = bulletSprite;
                    ammoType = new AT9mm();
                    flare = new SpriteMap("smallFlare", 11, 10);
                    flare.center = new Vec2(0f, 5f);
                    break;

                case 1:
                    gunSprite = laserSprite;
                    ammoType = new ATLaser();
                    flare = new SpriteMap("laserFlare", 16, 16);
                    flare.center = new Vec2(0f, 8f);
                    break;

                case 2:
                    gunSprite = rocketSprite;
                    ammoType = new ATMissile();
                    flare = new SpriteMap("smallFlare", 11, 10);
                    flare.center = new Vec2(0f, 5f);
                    break;
            }
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
            {
                FollowCam followCam = Level.current.camera as FollowCam;
                if (followCam != null)
                    followCam.Add(this);
            }

            ChangeAmmo();

            base.Initialize();
        }

        public override void Terminate()
        {
            FollowCam followCam = Level.current.camera as FollowCam;
            if (followCam != null)
                followCam.Remove(this);

            base.Terminate();
        }

        public override void Update()
        {
            if (_flareAlpha > 0f)
                _flareAlpha -= 0.5f;
            else
                _flareAlpha = 0.0f;

            float ang = angle + (offDir < 0 ? (float)Math.PI : 0f);

            foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(position + 180f * new Vec2((float)Math.Cos(ang), (float)Math.Sin(ang)), 152f))
            {
                IList<Block> blockers = new List<Block>();
                foreach (Block b in Level.CheckLineAll<Block>(position, physicsObject.position))
                {
                    if (!(b is Window))
                        blockers.Add(b);
                }
                if (physicsObject != this
                    && physicsObject != owner
                    && physicsObject.owner == null
                    && (physicsObject is Duck
                    || (physicsObject is RagdollPart && ((RagdollPart)physicsObject)._doll != null && ((RagdollPart)physicsObject)._doll._duck != null && !((RagdollPart)physicsObject)._doll._duck.dead)
                    || physicsObject is TrappedDuck
                    || physicsObject is ISentryTarget && !((ISentryTarget)physicsObject).IsDead)
                    && blockers.Count == 0
                    && (_target == null || (position - physicsObject.position).length < (position - _target.position).length))
                    _target = physicsObject;
            }

            if (_target != null)
            {
                IList<Block> blockers = new List<Block>();
                foreach (Block b in Level.CheckLineAll<Block>(position, _target.position))
                {
                    if (!(b is Window))
                        blockers.Add(b);
                }
                if (!Level.CheckCircleAll<PhysicsObject>(position + 180f * new Vec2((float)Math.Cos(ang), (float)Math.Sin(ang)), 152f).Contains(_target) || blockers.Count > 0)
                    _target = null;
                else
                {
                    _gunAngle = MathHelper.Lerp(_gunAngle, (float)Math.Atan((_target.y - firePosition.y) / (_target.x - firePosition.x)), 0.1f);

                    if (_cooldown < rampup)
                    {
                        _cooldown += 1f / 60f;
                        if (isServerForObject && !_playedBeep)
                        {
                            netSFX_beep.Play();
                            _playedBeep = true;
                        }
                    }
                    else if (_shotsFired < shots)
                    {
                        _ramped = true;
                        if (isServerForObject)
                        {
                            Fire();
                            Send.Message(new NMFireGun(null, firedBullets, (byte)0, false, (byte)4, true), NetMessagePriority.Urgent);
                            firedBullets.Clear();
                        }
                        _shotsFired++;
                        _cooldown = rampup - delay;
                    }
                    else
                    {
                        _shotsFired = 0;
                        _cooldown = 0f;
                        _ramped = false;
                    }
                }
            }
            else
            {
                _gunAngle = MathHelper.Lerp(_gunAngle, angle, 0.1f);
                _cooldown = 0f;
                _shotsFired = 0;
                _ramped = false;
                _playedBeep = false;
            }

            _laserSight = _target != null;

            if (_ramped)
            {
                if (!gunSprite.currentAnimation.Equals("firing"))
                    gunSprite.SetAnimation("firing");
            }
            else if (!gunSprite.currentAnimation.Equals("idle"))
                gunSprite.SetAnimation("idle");

            base.Update();
        }

        private void Fire()
        {
            firedBullets.Clear();
            float fireAngleDegrees = Maths.RadToDeg(_gunAngle);
            float fireAngle = offDir >= 0 ? fireAngleDegrees + ammoType.barrelAngleDegrees : fireAngleDegrees + 180f - ammoType.barrelAngleDegrees;
            Bullet bullet = ammoType.FireBullet(firePosition, owner, fireAngle, this);
            if (Network.isActive && isServerForObject)
                firedBullets.Add(bullet);
            _flareAlpha = 1.5f;
            PlayFireSound();
        }

        private void PlayFireSound()
        {
            if (isServerForObject)
            {
                switch (shotType)
                {
                    default:
                        netSFX_bullet.Play();
                        break;

                    case 1:
                        netSFX_laser.Play();
                        break;

                    case 2:
                        netSFX_missile.Play();
                        break;
                }
            }
        }

        public override void Draw()
        {
            _gunAngle %= (2*(float)Math.PI);
            flare.flipH = gunSprite.flipH = offDir < 0;
            flare.angle = gunSprite.angle = _gunAngle;
            Graphics.Draw((Sprite)gunSprite, x + (offDir * 5f), y - 6f, 0.6f);

            if (_flareAlpha > 0f)
                Graphics.Draw((Sprite)flare, firePosition.x, firePosition.y);

            if (_laserSight)
            {
                ATTracer atTracer = new ATTracer();
                atTracer.range = 2000f;
                
                Vec2 laserPosition = firePosition;
                atTracer.penetration = 0.4f;
                float fireAngleDegrees = Maths.RadToDeg(_gunAngle);
                float fireAngle = offDir >= 0 ? fireAngleDegrees + ammoType.barrelAngleDegrees : fireAngleDegrees + 180f - ammoType.barrelAngleDegrees;
                _wallPoint = new Bullet(laserPosition.x, laserPosition.y, atTracer, -fireAngle, owner, false, -1f, true).end;
                _laserInit = true;
            }

            base.Draw();
        }

        public override void DrawGlow()
        {
            if (_laserSight && _laserInit)
            {
                Vec2 laserPosition = firePosition;
                float length = (laserPosition - _wallPoint).length;
                float val1 = 100f;
                if (ammoType != null)
                    val1 = ammoType.range;
                Vec2 normalized = (_wallPoint - laserPosition).normalized;
                Vec2 vec2 = laserPosition + normalized * Math.Min(val1, length);
                Graphics.DrawTexturedLine(_laserTex, laserPosition, vec2, Color.Red, 0.5f, depth - 1);
                if (length > val1)
                {
                    for (int index = 1; index < 4; ++index)
                    {
                        Graphics.DrawTexturedLine(_laserTex, vec2, vec2 + normalized * 2f, Color.Red * (float)(1f - index * 0.2f), 0.5f, depth - 1);
                        vec2 += normalized * 2f;
                    }
                }
                else
                {
                    _sightHit.alpha = 1f;
                    _sightHit.color = Color.Red;
                    Graphics.Draw(_sightHit, _wallPoint.x, _wallPoint.y);
                }
            }

            base.DrawGlow();
        }

        public override void CheckIfHoldObstructed()
        {
            if (duck != null)
                duck.holdObstructed = false;
        }
    }
}
