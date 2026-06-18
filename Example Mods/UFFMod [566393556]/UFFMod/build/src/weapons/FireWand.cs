namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|misc")]
    public class FireWand : Wand
    {
        public StateBinding _cooldownStateBinding = new StateBinding("_cooldown");
        public StateBinding netSFX_flameStateBinding = new NetSoundBinding("netSFX_flame");

        public NetSoundEffect netSFX_flame = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\firewandnoise")
        })
        {
            volume = 0.25f
        };

        public int _cooldown;

        public FireWand(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Fire Wand";

            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\fireWand"), 18, 25);
            sprite.AddAnimation("SCHWING", 0.6f, false, 2, 3, 4, 0);
            sprite.AddAnimation("held", 1f, false, 1);
            sprite.AddAnimation("unheld", 1f, false, 0);
            graphic = sprite;

            // weapon settings
            ammo = 80;
            flammable = 0.4f;
            _hasTrigger = false;
            physicsMaterial = PhysicsMaterial.Wood;

            // defaults
            _currentState = 0;
            _cooldown = 0;
            skipFinish = true;
        }

        public override void Update()
        {
            if (_currentState == 1 && isServerForObject)
            {
                if (ammo <= 0)
                {
                    if (sprite.finished)
                        _currentState = 0;
                }
                else
                {
                    if(this != null)
                        Level.Add(new FireSprinkle(Offset(barrelOffset).x, Offset(barrelOffset).y, Rando.Float(2.5f, 3.5f), (offDir > 0 ? angleDegrees : angleDegrees + 180f) + Rando.Float(-12f, 12f), this));
                    if (_cooldown == 0)
                    {
                        _cooldown = 3;
                        ammo--;
                        netSFX_flame.Play();
                    }
                    else
                        _cooldown--;
                }
            }

            if (sprite.finished && _currentState == -1)
                _currentState = 0;

            base.Update();
        }

        public override void Draw()
        {
            if (_currentState == 0)
            {
                if (owner == null)
                    sprite.SetAnimation("unheld");
                else
                    sprite.SetAnimation("held");
            }
            base.Draw();
        }

        public override void CheckIfHoldObstructed()
        {
            if (owner == null || _currentState == 0)
                return;
            duck.holdObstructed = false;
        }

        public override void OnReleaseAction()
        {
            _currentState = -1;
            base.OnReleaseAction();
        }

        public override void Fire()
        {
            // do nothing
        }
    }
}
