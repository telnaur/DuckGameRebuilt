using System.Linq;

namespace DuckGame.HaloWeapons
{
    public sealed class CreditIncreaseDisplay : CreditDisplay
    {
        private readonly int[] _increments = new int[]
        {
            10, 
            50,
            100
        };

        private int _currentCredits;
        private float _pauseTimer = 0.5f;
        private float _fontScaleChangeSpeed = -0.2f;

        private bool _remove;

        public CreditIncreaseDisplay(int initialCredits, float x, float y) : base(x, y)
        {
            _currentCredits = initialCredits;
        }

        protected override int DisplayedValue => _currentCredits;
        private int DestinationCredits => Skins.Credits;

        public override void Update()
        {
            base.Update();

            if (_pauseTimer > 0f)
            {
                _pauseTimer -= 0.01f;
                return;
            }

            if (_remove)
            {
                Level.Remove(this);
                return;
            }

            Vec2 scale = Font.scale;
            Font.scale = new Vec2(scale.x + _fontScaleChangeSpeed, 1f);

            float scaleX = Font.scale.x;

            if (scaleX <= 0f)
            {
                _currentCredits += GetCreditsIncrement();
                ToggleSpeed();
            }
            else if (scaleX >= 1f)
            {
                Font.scale = new Vec2(1f);
                ToggleSpeed();

                if (_currentCredits >= Skins.Credits)
                {
                    _pauseTimer = 0.8f;
                    _remove = true;

                    return;
                }

                _pauseTimer = 0.2f;
            }
        }

        private int GetCreditsIncrement()
        {
            int difference = DestinationCredits - _currentCredits;

            foreach (int increment in _increments.OrderByDescending(increment => increment))
                if (increment <= difference)
                    return increment;

            return difference;
        }

        private void ToggleSpeed()
        {
            _fontScaleChangeSpeed *= -1f;
        }
    }
}
