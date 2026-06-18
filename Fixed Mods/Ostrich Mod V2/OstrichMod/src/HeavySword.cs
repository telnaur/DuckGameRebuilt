using System;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Eternal")]
    public class HeavySword : Gun
    {
        public StateBinding _swingBinding = new StateBinding("_swing", -1, false, false);

        private SpriteMap _sprite;

        private SpriteMap _sledgeSwing;

        private Vec2 _offset = default(Vec2);

        private float _swing;

        private float _swingLast;

        private float _swingVelocity;

        private float _swingForce;

        private bool _pressed;

        private float _lastSpeed;

        private int _lastDir;

        private float _fullSwing;

        private float _sparkWait;

        private bool _swung;

        private bool _drawOnce;

        private bool _held;

        private PhysicsObject _lastOwner;

        private float _hPull;

        public HeavySword(float xval, float yval)
        : base(xval, yval)
        {
            base.ammo = 4;
            base._ammoType = new ATLaser();
            base._ammoType.range = 170f;
            base._ammoType.accuracy = 0.8f;
            base._type = "gun";
            base._holdOffset = new Vec2(2f, -1f);
            _sprite = new SpriteMap(GetPath("HeavySword"), 11, 32);
            _sledgeSwing = new SpriteMap("sledgeSwing", 32, 32, false);
            _sledgeSwing.AddAnimation("swing", 0.8f, false, 0, 1, 2, 3, 4, 5);
            _sledgeSwing.currentAnimation = "swing";
            _sledgeSwing.speed = 0f;
            _sledgeSwing.center = new Vec2(16f, 16f);
            graphic = _sprite;
            center = new Vec2(5f, 28f);
            collisionOffset = new Vec2(-5f, -28f);
            collisionSize = new Vec2(8f, 32f);
            base._barrelOffsetTL = new Vec2(0f, 0f);
            base._fireSound = "smg";
            base._fullAuto = true;
            base._fireWait = 1f;
            base._kickForce = 3f;
            base._dontCrush = false;
            weight = 6f;
            base.collideSounds.Add("rockHitGround2");
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is IPlatform)
            {
                for (int i = 0; i < 4; i++)
                {
                    Level.Add(Spark.New(base.barrelPosition.x + Rando.Float(-6f, 6f), base.barrelPosition.y + Rando.Float(-3f, 3f), -MaterialThing.ImpactVector(from), 0.02f));
                }
            }
        }

        public override void CheckIfHoldObstructed()
        {
            Duck duckOwner = owner as Duck;
            if (duckOwner != null)
            {
                duckOwner.holdObstructed = false;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void ReturnToWorld()
        {
            _sprite.frame = 0;
            _swing = 0f;
            _swingForce = 0f;
            _pressed = false;
            _swung = false;
            _fullSwing = 0f;
            _swingVelocity = 0f;
        }

        public override void Update()
        {
            if (_lastOwner != null && owner == null)
            {
                _lastOwner.frictionMod = 0f;
                _lastOwner = null;
            }
            _swingVelocity = Maths.LerpTowards(_swingVelocity, _swingForce, 0.1f);
            Duck duckOwner = owner as Duck;
            if (base.isServerForObject)
            {
                _swing += _swingVelocity;
                float dif = _swing - _swingLast;
                _swingLast = _swing;
                if (_swing > 1f)
                {
                    _swing = 1f;
                }
                if (_swing < 0f)
                {
                    _swing = 0f;
                }
                _sprite.flipH = false;
                _sprite.flipV = false;
                if (_sparkWait > 0f)
                {
                    _sparkWait -= 0.1f;
                }
                else
                {
                    _sparkWait = 0f;
                }
                if (duckOwner != null)
                {
                    float spd = duckOwner.hSpeed;
                    _hPull = Maths.LerpTowards(_hPull, duckOwner.hSpeed, 0.15f);
                    if (Math.Abs(duckOwner.hSpeed) < 0.1f)
                    {
                        _hPull = 0f;
                    }
                    float fricDif = Math.Abs(duckOwner.hSpeed - _hPull);
                    duckOwner.frictionMod = 0f;
                    if (duckOwner.hSpeed > 0f && _hPull > duckOwner.hSpeed)
                    {
                        duckOwner.frictionMod = (0f - fricDif) * 3f;
                    }
                    if (duckOwner.hSpeed < 0f && _hPull < duckOwner.hSpeed)
                    {
                        duckOwner.frictionMod = (0f - fricDif) * 3f;
                    }
                    _lastDir = duckOwner.offDir;
                    _lastSpeed = spd;
                    if (_swing != 0f && dif > 0f)
                    {
                        duckOwner.hSpeed += (float)duckOwner.offDir * (dif * 3f) * base.weightMultiplier;
                        duckOwner.vSpeed -= dif * 2f * base.weightMultiplier;
                    }
                }
            }
            if (_swing < 0.5f)
            {
                float norm2 = _swing * 2f;
                _sprite.imageIndex = (int)(norm2 * 10f);
                _sprite.angle = 1.2f - norm2 * 1.5f;
                _sprite.yscale = 1f - norm2 * 0.1f;
            }
            else if (_swing >= 0.5f)
            {
                float norm = (_swing - 0.5f) * 2f;
                _sprite.imageIndex = 10 - (int)(norm * 10f);
                _sprite.angle = -0.3f - norm * 1.5f;
                _sprite.yscale = 1f - (1f - norm) * 0.1f;
                _fullSwing += 0.16f;
                if (!_swung)
                {
                    _swung = true;
                    if (base.duck != null && base.isServerForObject)
                    {
                        Level.Add(new ForceWave(base.x + (float)offDir * 4f + owner.hSpeed, base.y + 8f, offDir, 0.15f, 4f + Math.Abs(owner.hSpeed), owner.vSpeed, base.duck));
                    }
                }
            }
            if (_swing == 1f)
            {
                _pressed = false;
            }
            if (_swing == 1f && !_pressed && _fullSwing > 1f)
            {
                _swingForce = -0.08f;
                _fullSwing = 0f;
            }
            if (_sledgeSwing.finished)
            {
                _sledgeSwing.speed = 0f;
            }
            _lastOwner = (owner as PhysicsObject);
            if (base.duck != null)
            {
                if (base.duck.action && !_held && _swing == 0f)
                {
                    _fullSwing = 0f;
                    duckOwner._disarmDisable = 30;
                    duckOwner.crippleTimer = 1f;
                    _sledgeSwing.speed = 1f;
                    _sledgeSwing.frame = 0;
                    _swingForce = 0.6f;
                    _pressed = true;
                    _swung = false;
                    _held = true;
                }
                if (!base.duck.action)
                {
                    _pressed = false;
                    _held = false;
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            if (owner != null && _drawOnce)
            {
                _offset = new Vec2((float)offDir * -6f + _swing * 5f * (float)offDir, -3f + _swing * 5f);
                Vec2 pos = base.position + _offset;
                graphic.position = pos;
                graphic.depth = base.depth;
                Duck duckOwner = owner as Duck;
                base.handOffset = new Vec2(_swing * 3f, 0f - _swing * 4f);
                base.handAngle = 1.4f + (_sprite.angle * 0.5f - 1f);
                if (duckOwner != null && duckOwner.offDir < 0)
                {
                    _sprite.angle = 0f - _sprite.angle;
                    base.handAngle = 0f - base.handAngle;
                }
                graphic.Draw();
                if (_sledgeSwing.speed > 0f)
                {
                    if (duckOwner != null)
                    {
                        _sledgeSwing.flipH = ((byte)((duckOwner.offDir <= 0) ? 1 : 0) != 0);
                    }
                    _sledgeSwing.position = base.position;
                    _sledgeSwing.depth = base.depth + 1;
                    _sledgeSwing.Draw();
                }
            }
            else
            {
                base.Draw();
                _drawOnce = true;
            }
        }

        public override void OnPressAction()
        {
        }

        public override void OnReleaseAction()
        {
        }

        public override void Fire()
        {
        }
    }
}
