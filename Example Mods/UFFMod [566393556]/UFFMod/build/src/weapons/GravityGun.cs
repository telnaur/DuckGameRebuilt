namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|tech")]
    [BaggedProperty("isSuperWeapon", true)]
    public class GravityGun : Gun
    {
        public StateBinding _theHoleStateBinding = new StateBinding("_theHole");
        public StateBinding _spriteSpeedStateBinding = new StateBinding("_spriteSpeed");
        public StateBinding _chargeStateBinding = new StateBinding("_charge");
        public StateBinding _currentStateStateBinding = new StateBinding("_currentState");
        public StateBinding netSFX_whargarbleStateBinding = new NetSoundBinding("netSFX_whargarble");

        public NetSoundEffect netSFX_whargarble = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\whargarble")
        });

        public BlackHole _theHole;
        public float _spriteSpeed;
        public int _charge;
        public int _currentState;

        private SpriteMap sprite;

        public GravityGun(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Gravity Gun";

            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\gravitygun"), 24, 22);
            sprite.AddAnimation("rotate", 1f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            sprite.SetAnimation("rotate");
            sprite.frame = 0;
            graphic = sprite;
            _center = new Vec2(12f, 11f);
            _collisionSize = new Vec2(12f, 12f);
            _collisionOffset = new Vec2(-6f, -5f);
            _holdOffset = new Vec2(4f, -1f);
            _barrelOffsetTL = new Vec2(29f, 10f);

            // weapon settings
            ammo = 1;
            _kickForce = 5f;
            _weight = 4f;

            // defaults
            _spriteSpeed = 0f;
            _charge = 0;
            _currentState = 0;
        }

        public override void Terminate()
        {
            if (isServerForObject && _theHole != null && !_theHole._released && !_theHole._vanish)
                _theHole._vanish = true;
            base.Terminate();
        }

        public override void Update()
        {
            if (isServerForObject)
            {
                if (_currentState == 1)
                {
                    if (_charge < 100)
                        _charge++;
                    else
                        _currentState = 2;
                }
                else if (_currentState == 0)
                    _charge = 0;
            }

            if (_theHole != null && !_theHole._released)
            {
                _theHole.position = Offset(barrelOffset);
                _theHole.xscale = _theHole.yscale = _charge * 0.2f / 100;
            }

            if (_currentState == 0 && _theHole != null && !_theHole._released && !_theHole._vanish)
                _theHole._vanish = true;

            if (_currentState == 0 && _spriteSpeed > 0f)
                _spriteSpeed -= 0.025f;
            if (_currentState != 0 && _spriteSpeed < 0.5f)
                _spriteSpeed += 0.025f;

            base.Update();
        }

        public override void Draw()
        {
            sprite.speed = _spriteSpeed;

            base.Draw();
        }

        public override void OnPressAction()
        {
            if (_currentState == 0)
            {
                if (isServerForObject)
                {
                    _theHole = new BlackHole(Offset(barrelOffset).x, Offset(barrelOffset).y, this);
                    Level.Add(_theHole);
                }
                _currentState = 1;
            }
        }

        public override void OnReleaseAction()
        {
            if (_currentState == 2 && owner != null)
                 Fire();
            else
                _theHole._vanish = true;

            _currentState = 0;
        }

        public override void Fire()
        {
            ApplyKick();

            if (isServerForObject)
            {
                netSFX_whargarble.Play();
                _theHole._fireSpeed = 1f;
                _theHole._fireAngle = offDir >= 0 ? angleDegrees : angleDegrees + 180f;
                _theHole._released = true;

                _charge = 0;
                _currentState = 0;
            }
        }
    }
}
