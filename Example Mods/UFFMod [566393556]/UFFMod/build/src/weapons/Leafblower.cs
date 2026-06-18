namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|misc")]
    [BaggedProperty("isFatal", false)]
    public class Leafblower : Gun
    {
        public StateBinding _gustStateBinding = new StateBinding("_gust");
        public StateBinding _isFiringStateBinding = new StateBinding("_isFiring");
        public StateBinding _ammoCooldownStateBinding = new StateBinding("_ammoCooldown");

        public LeafblowerGust _gust;
        public bool _isFiring;
        public int _ammoCooldown;

        private SpriteMap sprite;

        public Leafblower(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\Leafblower"), 18, 9);
            graphic = sprite;
            _center = new Vec2(9f, 5f);
            _collisionSize = new Vec2(10f, 6f);
            _collisionOffset = new Vec2(-5f, -3f);
            _holdOffset = new Vec2(2, 4);

            // weapon settings
            ammo = 60;
            _weight = 6f;
            physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void Terminate()
        {
            if (_gust != null)
                Level.Remove(_gust);
            base.Terminate();
        }

        public override void CheckIfHoldObstructed()
        {
            if (owner != null)
                duck.holdObstructed = false;
        }

        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                if (_gust == null && isServerForObject)
                {
                    _gust = new LeafblowerGust(x + (float)offDir * 32f, y, this);
                    Level.Add(_gust);
                }
                _isFiring = true;
            }
            else
                SFX.Play(_clickSound, 1f, 0.0f, 0.0f, false);
            base.OnPressAction();
        }

        public override void OnReleaseAction()
        {
            _isFiring = false;
            base.OnReleaseAction();
        }

        public override void Update()
        {
            if (_ammoCooldown > 0)
                _ammoCooldown--;

            if (_isFiring)
            {
                if (_ammoCooldown == 0)
                {
                    if (ammo == 0)
                    {
                        _isFiring = false;
                        SFX.Play(_clickSound, 1f, 0.0f, 0.0f, false);
                    }
                    else
                    {
                        SFX.Play(Mod.GetPath<UffMod>("SFX\\youcanblowmyleaf"), 1.5f, Rando.Float(0.15f), 0f, false);
                        ammo--;
                    }
                    _ammoCooldown = 8;
                }
                else
                    _ammoCooldown--;
            }

            base.Update();
        }

        public override void Fire()
        {
            // do nothing
        }
    }
}
