using System;

namespace DuckGame.HaloWeapons
{
    public sealed class CreditFlyAwayDisplay : CreditDisplay
    {
        private readonly float _initialX;
        private readonly int _value;

        private float _maxOffset;
        private float _waveOffset;
        private float _previousSinWaveValue;

        public CreditFlyAwayDisplay(int value, float x, float y) : base(x, y)
        {
            _initialX = x;
            _value = value;

            position = new Vec2(x, y);
            depth = 2.9f;
            alpha = 0.7f;

            DrawCoin = false;
        }

        protected override int DisplayedValue => _value;

        public override void Update()
        {
            y -= 0.5f;

            if (y < -Font.characterHeight)
            {
                Level.Remove(this);
                return;
            }

            float sinWaveValue = (float)Math.Sin(_waveOffset);

            if (sinWaveValue * _previousSinWaveValue <= 0f)
                _maxOffset = Rando.Float(5f, 10f);

            position.x = _initialX + _maxOffset * sinWaveValue;
            _waveOffset += 0.5f / _maxOffset;

            _previousSinWaveValue = sinWaveValue;
        }
    }
}
