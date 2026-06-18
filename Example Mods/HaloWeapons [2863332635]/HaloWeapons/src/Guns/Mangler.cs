using System;

namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Guns)]
    [GunGameLevel(7)]
    public class Mangler : HaloWeapon
    {
        private readonly Vec2 _spikeOffset = new Vec2(7f, 5f);
        private readonly float _maxPunchOffset = 8f;
        private readonly float _maxHandAngle = Maths.PI / 15f;
        private readonly float _initialHoldOffsetX;

        [Binding] private float _punchOffsetMultiplier;
        [Binding] private int _spriteFrame;

        private float _holdTimer;
        private float _punchForce;
        private bool _punching;

        public Mangler(float x, float y) : base(x, y)
        {
            ammo = 5;
            loseAccuracy = 0.2f;
            maxAccuracyLost = 0.4f;

            _fireSound = Paths.GetSoundPath("manglerFire.wav");
            _ammoType = new ATMangler();
            _holdOffset = new Vec2(2f, 3f);
            _initialHoldOffsetX = _holdOffset.x;
            _barrelOffsetTL = new Vec2(17f, 5f);
            _fireWait = 3f;
            _kickForce = 3f;
        }

        private bool CanPunch => duck is not null && !duck._hovering && !duck.holdObstructed;

        public override void Update()
        {
            base.Update();

            if (_punching)
            {
                if (CanPunch)
                {
                    if (_holdTimer > 0f)
                    {
                        _holdTimer -= 0.01f;
                    }
                    else
                    {
                        _punchOffsetMultiplier += _punchForce;

                        if (_punchOffsetMultiplier >= 1f)
                        {
                            _punchForce *= -1f;
                            _holdTimer = 0.25f;
                        }
                        else if (_punchOffsetMultiplier <= 0f)
                        {
                            _punchOffsetMultiplier = 0f;
                            _punching = false;
                        }
                    }

                    foreach (MaterialThing thing in Level.CheckCircleAll<MaterialThing>(Offset(_spikeOffset), 8f))
                    {
                        if (thing == owner || thing == this)
                            continue;

                        thing.Destroy(new DTImpact(this));
                    }
                }
                else
                {
                    ResetValues();
                }
            }

            float offset = _maxPunchOffset * _punchOffsetMultiplier;

            handOffset.x = offset;
            _holdOffset.x = _initialHoldOffsetX + offset;

            handAngle = _maxHandAngle * _punchOffsetMultiplier * -offDir;

            handOffset.y = (float)Math.Tan(handAngle) * offset * offDir;

            SpriteMap.frame = _spriteFrame;
        }

        public override void OnPressAction()
        {
            if (_punching || _wait > 0f)
                return;

            if (ammo > 0)
            {
                base.OnPressAction();

                if (isServerForObject)
                    _spriteFrame++;
            }
            else if (isServerForObject && CanPunch)
            {
                _punching = true;
                _punchForce = 0.25f;

                duck.quack = 35;
                SFX.PlaySynchronized("quack", Rando.Float(0.8f, 1f), Rando.Float(0.2f), 0f, false);
            }
        }

        public override void Thrown()
        {
            base.Thrown();

            ResetValues();
        }

        protected override Sprite CreateGraphics(string spritePath)
        {
            return new SpriteMap(spritePath, 18, 17);
        }

        private void ResetValues()
        {
            _punchOffsetMultiplier = 0f;
            _holdTimer = 0f;
            _punchForce = 0f;
            _punching = false;
        }
    }
}

