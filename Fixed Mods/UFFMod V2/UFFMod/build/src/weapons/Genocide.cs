namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|explosives")]
    [BaggedProperty("isSuperWeapon", true)]
    public class Genocide : Gun
    {
        public StateBinding _currentStateStateBinding = new StateBinding("_currentState");
        public StateBinding _animationIndexStateBinding = new StateBinding("netAnimationIndex");
        public StateBinding _frameStateBinding = new StateBinding("spriteFrame");
        public StateBinding netSFX_beep1StateBinding = new NetSoundBinding("netSFX_beep1");
        public StateBinding netSFX_beep2StateBinding = new NetSoundBinding("netSFX_beep2");
        public StateBinding netSFX_beep3StateBinding = new NetSoundBinding("netSFX_beep3");
        public StateBinding netSFX_beep4StateBinding = new NetSoundBinding("netSFX_beep4");
        public StateBinding netSFX_beep5StateBinding = new NetSoundBinding("netSFX_beep5");

        public NetSoundEffect netSFX_beep1 = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\doyouwanttohaveabadtime")
        })
        {
            volume = 1f,
            pitch = -0.2f
        };
        public NetSoundEffect netSFX_beep2 = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\doyouwanttohaveabadtime")
        })
        {
            volume = 1f,
            pitch = -0.1f
        };
        public NetSoundEffect netSFX_beep3 = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\doyouwanttohaveabadtime")
        })
        {
            volume = 1f,
            pitch = 0f
        };
        public NetSoundEffect netSFX_beep4 = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\doyouwanttohaveabadtime")
        })
        {
            volume = 1f,
            pitch = 0.1f
        };
        public NetSoundEffect netSFX_beep5 = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\doyouwanttohaveabadtime")
        })
        {
            volume = 1f,
            pitch = 0.2f
        };

        public int _currentState;

        private SpriteMap sprite;

        private byte netAnimationIndex
        {
            get
            {
                if (sprite == null)
                    return (byte)0;
                return (byte)sprite.animationIndex;
            }
            set
            {
                if (sprite == null || sprite.animationIndex == (int)value)
                    return;
                sprite.animationIndex = (int)value;
            }
        }

        public byte spriteFrame
        {
            get
            {
                if (sprite == null)
                    return (byte)0;
                return (byte)sprite._frame;
            }
            set
            {
                if (sprite == null)
                    return;
                sprite._frame = (int)value;
            }
        }

        public Genocide(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\genocide"), 21, 20);
            sprite.AddAnimation("let's carnage the world", 0.0333f, false, 1, 2, 3, 4, 5);
            sprite.AddAnimation("normal", 1f, false, 0);
            graphic = sprite;
            _center = new Vec2(12f, 10f);
            _collisionSize = new Vec2(14f, 14f);
            _collisionOffset = new Vec2(-7f, -7f);
            _holdOffset = new Vec2(5f, -3f);

            // weapon settings
            ammo = 1;
            _fireSound = Mod.GetPath<UffMod>("SFX\\becauseyou'regoingtohaveabadtime.wav");
            _kickForce = 15f;
            _weight = 9f;

            // defaults
            _currentState = 0;
        }

        public override void Update()
        {
            if (_currentState != 0 && sprite.frame == _currentState)
            {
                if (isServerForObject)
                {
                    switch (sprite.frame)
                    {
                        default:
                            break;

                        case 1:
                            netSFX_beep2.Play();
                            break;

                        case 2:
                            netSFX_beep3.Play();
                            break;

                        case 3:
                            netSFX_beep4.Play();
                            break;

                        case 4:
                            netSFX_beep5.Play();
                            break;
                    }
                }
                _currentState++;
            }

            if (sprite.finished && _currentState == 5)
                Fire();

            base.Update();
        }

        public override void Draw()
        {
            if (_currentState <= 0)
                sprite.SetAnimation("normal");
            base.Draw();
        }

        public override void OnPressAction()
        {
            if (_currentState == 0)
            {
                sprite.SetAnimation("let's carnage the world");
                if (isServerForObject)
                    netSFX_beep1.Play();
                _currentState = 1;
            }
        }

        public override void OnReleaseAction()
        {
            if (_currentState >= 1)
            {
                sprite.SetAnimation("normal");
                _currentState = 0;
            }
        }

        public override void Fire()
        {
            ApplyKick();
            PlayFireSound();
            // ammo--; // disable for infinite ammo

            if (isServerForObject)
                for (int i = 0; i < 32; i++)
                    Level.Add(new GenocideOrb(x + (offDir * 10f), y + Rando.Float(0f, 4f), Rando.Float(6f, 12f), offDir > 0 ? Rando.Float(-20f, 20f) : Rando.Float(-20f, 20f) + 180f));

            _currentState = 0; // enable for infinite ammo
            // _currentState = ammo == 0 ? -1 : 0; // disable for infinite ammo
        }

        protected override void PlayFireSound()
        {
            SFX.Play(_fireSound, 1f, 0, 0f, false);
        }

        public override void CheckIfHoldObstructed()
        {
            if (duck == null || _currentState == 0)
                return;
            duck.holdObstructed = false;
        }
    }
}
