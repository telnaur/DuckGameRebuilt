using System;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|guns")]
    [BaggedProperty("isSuperWeapon", true)]
    public class SixShooter : Gun
    {
        public StateBinding _fireTimeStateBinding = new StateBinding("_fireTime");
        public StateBinding _flareAlphaStateBinding = new StateBinding("_flareAlpha");
        public StateBinding loseAccuracyLostStateBinding = new StateBinding("loseAccuracy");
        public StateBinding maxAccuracyLostStateBinding = new StateBinding("maxAccuracyLost");
        public StateBinding _accuracyLostStateBinding = new StateBinding("_accuracyLost");
        public StateBinding netSFX_magnumStateBinding = new NetSoundBinding("netSFX_magnum");

        public NetSoundEffect netSFX_magnum = new NetSoundEffect(new string[1]
        {
            "magnum"
        });

        public int _fireTime;

        public SixShooter(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // editor name
            _editorName = "Six Shooter";

            // collision & sprite settings
            graphic = (Sprite)new SpriteMap(Mod.GetPath<UffMod>("weapons\\highNoon"), 22, 10);
            center = new Vec2(11f, 5f);
            collisionOffset = new Vec2(-10f, -5f);
            collisionSize = new Vec2(20f, 10f);
            _holdOffset = new Vec2(3f, -1f);
            _barrelOffsetTL = new Vec2(21f, 2f);

            // weapon settings
            ammo = 6;
            _ammoType = new ATMagnum();
            _kickForce = 1.2f;
            _fireSound = "magnum";
            _fireWait = 3f;
        }

        public override void Update()
        {
            if (_fireTime >= 10)
            {
                if (owner != null)
                {
                    loseAccuracy = 0.4f;
                    maxAccuracyLost = 0.8f;
                    if (_fireWait > 0f)
                        _fireWait = 0f;

                    if (_fireTime > 10)
                        _fireTime--;
                    else
                    {
                        if (ammo > 0)
                        {
                            _fireTime = 15;
                            if (isServerForObject)
                            {
                                Fire();
                                Send.Message(new NMFireGun(this, firedBullets, bulletFireIndex, false, duck != null ? duck.netProfileIndex : (byte)4, true), NetMessagePriority.Urgent);
                                firedBullets.Clear();
                            }
                        }
                        if (ammo == 0)
                            _fireTime = -1;
                    }
                }
                else
                {
                    loseAccuracy = 0f;
                    maxAccuracyLost = 0f;
                    _fireTime = 0;
                }
            }

            base.Update();
        }

        protected override void PlayFireSound()
        {
            if (_fireTime >= 10)
            {
                if (isServerForObject)
                {
                    netSFX_magnum.pitch = Rando.Float(0.2f) - 0.1f;
                    netSFX_magnum.Play();
                }
            }
            else
                SFX.Play(_fireSound, 1f, Rando.Float(0.2f) - 0.1f);
        }

        public override void OnPressAction()
        {
            // do nothing
        }

        public override void OnHoldAction()
        {
            if (ammo > 0 && _fireTime < 10)
                _fireTime++;
        }

        public override void OnReleaseAction()
        {
            if (_fireTime >= 0 && _fireTime < 10)
            {
                _fireTime = 0;
                Fire();
            }

            if (_fireTime == -1)
                _fireTime = 0;

            base.OnReleaseAction();
        }
    }
}
