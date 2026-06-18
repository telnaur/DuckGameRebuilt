using System;

namespace DuckGame.HaloWeapons
{
    public class EnergyBeam : Beam, IFadingThing
    {
        private Vec2? _previousWaveDrawPosition;
        private int _timer = 25;

        private float _waveStartOffset1 = 0f;
        private float _waveStartOffset2 = 0.3f;

        public EnergyBeam(float x, float y) : base(x, y)
        {
            Texture = Resources.LoadTexture("energyBeam.png");
            Penetration = 6f;
            Thickness = 0.4f;
            Range = 10000f;
        }

        public float Timer => _timer;
        public float MaxTime => 25;

        public override void Update()
        {
            base.Update();

            _waveStartOffset1 += 0.3f;
            _waveStartOffset2 += 0.1f;

            _timer--;
        }

        public override void Draw()
        {
            base.Draw();

            DrawSinWave(new Color(102, 131, 190), 0.2f, _waveStartOffset1);
            DrawSinWave(new Color(91, 128, 203), 0.1f, _waveStartOffset2);
        }

        private void DrawSinWave(Color color, float periodMultiplier, float offset)
        {
            for (float x = 0f; x <= CurrentLength; x += 5f)
            {
                Vec2 nextPosition = position + new Vec2(x, (float)Math.Sin(x * periodMultiplier - offset) * Thickness * 10f).Rotate(-angle, Vec2.Zero);

                if (_previousWaveDrawPosition is null)
                {
                    _previousWaveDrawPosition = nextPosition;
                    continue;
                }

                Graphics.DrawLine(_previousWaveDrawPosition.Value, nextPosition, color * alpha, depth: depth.value + 0.1f);

                _previousWaveDrawPosition = nextPosition;
            }

            _previousWaveDrawPosition = null;
        }
    }
}
