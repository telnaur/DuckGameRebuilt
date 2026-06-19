namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|misc")]
    [BaggedProperty("isSuperWeapon", true)]
    public class CrateStrike : Gun
    {
        public StateBinding _hasFiredStateBinding = new StateBinding("_hasFired");
        public StateBinding _countdownStateBinding = new StateBinding("_countdown");
        public StateBinding netSFX_shhhhStateBinding = new NetSoundBinding("netSFX_shhhh");
        public StateBinding netSFX_sirenStateBinding = new NetSoundBinding("netSFX_siren");

        public NetSoundEffect netSFX_shhhh = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\shhhh")
        });
        public NetSoundEffect netSFX_siren = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\siren")
        });

        public bool _hasFired;
        public int _countdown;

        public EditorProperty<bool> carePackage = new EditorProperty<bool>(false, null, 0f, 1f, 1f, null, false, false);

        private SpriteMap crateSprite;
        private SpriteMap careSprite;
        private SpriteMap sprite;

        public CrateStrike(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Crate Strike";

            // collision & sprite settings
            crateSprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\crateStrike"), 17, 16);
            careSprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\carePackage"), 17, 16);
            sprite = crateSprite;
            graphic = sprite;
            _center = new Vec2(8f, 9f);
            _collisionSize = new Vec2(5f, 14f);
            _collisionOffset = new Vec2(-2f, -8f);
            _holdOffset = new Vec2(-2f, 0f);

            // weapon settings
            ammo = 1;
            flammable = 0f;
        }

        public override void CheckIfHoldObstructed()
        {
            if (owner == null)
                return;
            duck.holdObstructed = false;
        }

        public override void Draw()
        {
            if (carePackage)
                sprite = careSprite;
            else
                sprite = crateSprite;
            graphic = sprite;
            base.Draw();
        }

        public override void Update()
        {
            base.Update();

            if (_countdown > 1)
                _countdown--;
            if ((_countdown == 121 || _countdown == 61) && isServerForObject)
                netSFX_siren.Play(1f, 0f);
            if (_countdown == 1)
            {
                if (Network.isActive && isServerForObject) // whoosh sfx
                    netSFX_shhhh.Play(1f, 0f);
                else
                    SFX.Play(Mod.GetPath<UffMod>("SFX\\shhhh"));

                if (isServerForObject)
                {
                    if (carePackage)
                    {
                        if (Rando.Int(19) > 0)
                        {
                            Present present;
                            for (int i = 0; i < 5; i++)
                            {
                                if (System.Math.Abs(x - Level.current.camera.left) < System.Math.Abs(x - Level.current.camera.right))
                                {
                                    present = new Present(x - 32 + (i * 16), Level.current.camera.top - 256f - (i * 24));
                                    Level.Add(present);
                                    Fondle(present);
                                }
                                else
                                {
                                    present = new Present(x + 32 - (i * 16), Level.current.camera.top - 256f - (i * 24));
                                    Level.Add(present);
                                    Fondle(present);
                                }
                            }
                        }
                        else
                        {
                            Anvil anvil = new Anvil(x, Level.current.camera.top - 256f);
                            Level.Add(anvil);
                            Fondle(anvil);
                        }
                    }
                    else
                    {
                        Crate crate;
                        for (int i = 0; i < 11; i++)
                        {
                            if (System.Math.Abs(x - Level.current.camera.left) < System.Math.Abs(x - Level.current.camera.right))
                            {
                                crate = new Crate(x - 80 + (i * 16), Level.current.camera.top - 256f - (i * 24));
                                Level.Add(crate);
                                Fondle(crate);
                                crate.Burn(crate.position, this);
                            }
                            else
                            {
                                crate = new Crate(x + 80 - (i * 16), Level.current.camera.top - 256f - (i * 24));
                                Level.Add(crate);
                                Fondle(crate);
                                crate.Burn(crate.position, this);
                            }
                        }
                    }
                }

                ammo = 0;
                Level.Add(SmallSmoke.New(x, y));
                Level.Remove(this);
            }
        }

        public override void OnPressAction()
        {
            if (_hasFired)
                return;

            if (isServerForObject) // siren sfx
                netSFX_siren.Play();

            _countdown = 181;
            _hasFired = true;
        }

        public override void Fire()
        {
            // do nothing
        }
    }
}