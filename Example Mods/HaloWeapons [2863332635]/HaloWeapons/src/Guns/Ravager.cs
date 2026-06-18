using Microsoft.Xna.Framework.Audio;
using System;

namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Guns)]
    [GunGameLevel(8)]
    public class Ravager : BurstingWeapon
    {
        private readonly Sprite _chargeFlash = Resources.LoadSprite("ravagerCharge.png");
        private readonly Sound _chargeSound = Resources.LoadSound("ravagerCharge.wav");
        private readonly Sound _readySound = Resources.LoadSound("ravagerReady.wav", looped: true);

        private readonly SinWave _flashAlphaWave = new SinWave(0.03f, 1f);

        private readonly ATRavager _defaultAmmo = new ATRavager();
        private readonly ATRavagerCharged _chargedAmmo = new ATRavagerCharged();

        private readonly Vec2 _chargeFlashOffset = new Vec2(14.5f, 8f);
        private readonly string _defaultFireSound = Paths.GetSoundPath("ravagerFire.wav");
        private readonly string _chargedFireSound = Paths.GetSoundPath("ravagerFireCharged.wav");
        private readonly float _maxCharge = 0.95f;

        [Binding] private float _charge;
        [Binding] private bool _charging;

        public Ravager(float x, float y) : base(x, y)
        {
            MaxBulletsFired = 3;
            BurstDelay = 7;
            FireDelay = 0.5f;

            ammo = 15;

            _holdOffset = new Vec2(4f, 0f);
            _ammoType = _defaultAmmo;
            _barrelOffsetTL = new Vec2(23f, 6f);
            _fireSound = _defaultFireSound;

            _flare = Resources.LoadSpriteMap("ravagerFlare.png", 13, 7);
            _flare.center = new Vec2(-5f, _flare.height / 2f + 0.5f);

            _chargeFlash.alpha = 0f;
        }

        public override void Update()
        {
            base.Update();

            float alpha = _chargeFlash.alpha;

            if (_charging)
            {
                if (_charge >= _maxCharge)
                {
                    alpha = Math.Abs(_flashAlphaWave.value) * 0.7f + 0.2f;
                    _fireSound = _chargedFireSound;
                }
                else
                {
                    if (isServerForObject)
                        _charge += 0.01f;

                    alpha = GetChargeNormalized();
                    _fireSound = _defaultFireSound;
                }
            }
            else
            {
                alpha = Math.Max(alpha - 0.1f, 0f);
            }

            if (_charge >= _maxCharge)
            {
                if (_readySound.State != SoundState.Playing)
                {
                    _chargeSound.Stop();
                    _readySound.Play();
                }
            }
            else if (_charge > 0f)
            {
                if (_chargeSound.State != SoundState.Playing)
                    _chargeSound.Play();
            }

            _chargeFlash.alpha = alpha;
            _chargeFlash.flipH = offDir < 0;
            _chargeFlash.angle = angle;

            SpriteMap.frame = (int)(GetChargeNormalized() * 4f);
        }

        public override void OnPressAction()
        {
            if (isServerForObject && CanStartBursting)
            {
                if (ammo <= 0)
                    base.OnPressAction();
                else
                    _charging = true;
            }
        }

        public override void Fire()
        {
            base.Fire();

            if (_chargeSound.State == SoundState.Playing)
                _chargeSound.Stop();
            else
                _readySound.Stop();

            if (ammo > 0)
                _chargeFlash.alpha = 1f;
        }

        public override void OnReleaseAction()
        {
            if (ammo < 0 || !CanStartBursting)
                return;

            if (isServerForObject)
            {
                if (_charge >= _maxCharge)
                {
                    _ammoType = _chargedAmmo;

                    DoFireSynchronized();

                    _ammoType = _defaultAmmo;

                    ammo -= 2;
                }
                else
                {
                    Bursting = true;
                }
            }

            _charge = 0f;
            _charging = false;
            _flashAlphaWave.value = 1f;
        }

        public override void Draw()
        {
            base.Draw();

            Vec2 chargeFlashPosition = Offset(_barrelOffsetTL - _chargeFlashOffset);
            Graphics.Draw(_chargeFlash, chargeFlashPosition.x, chargeFlashPosition.y, depth.value - 0.1f);
        }

        protected override Sprite CreateGraphics(string spritePath)
        {
            return new SpriteMap(spritePath, 27, 12);
        }

        private float GetChargeNormalized()
        {
            return _charge / _maxCharge;    
        }
    }
}
