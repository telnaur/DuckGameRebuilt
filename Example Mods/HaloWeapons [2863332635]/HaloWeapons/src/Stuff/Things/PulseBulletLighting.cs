namespace DuckGame.HaloWeapons
{
    public class PulseBulletLighting : Thing, ITimerThing
    {
        private static readonly Color s_initialColor = new Color(64, 98, 230);
        private static readonly Color s_destinationColor = Color.White;

        private readonly PulseBullet _bullet;
        private readonly Vec2 _targetPosition;

        private float _timer = 1.5f;

        public PulseBulletLighting(PulseBullet bullet, Vec2 targetPosition)
        {
            _bullet = bullet;
            _targetPosition = targetPosition;
        }

        public float Timer => _timer;

        public static void Draw(Vec2 from, Vec2 to, float bulletDepth)
        {
            Color color = Lerp.Color(s_initialColor, s_destinationColor, Rando.Float(1f));
            Vec2 previousPosition = from;

            float angle = -Maths.PointDirectionRad(from, to);

            float maxDistance = Vec2.Distance(from, to);
            float distance = 0;

            bool doBreak = false;

            while (!doBreak)
            {
                Vec2 position;
                float length = Rando.Float(5f, 10f);

                if (distance + length >= maxDistance)
                {
                    position = to;
                    doBreak = true;
                }
                else
                {
                    position = previousPosition + new Vec2(length, Rando.Float(-2f, 2f)).Rotate(angle, Vec2.Zero);
                }

                Graphics.DrawLine(previousPosition, position, color, depth: bulletDepth - 0.1f);

                previousPosition = position;
                distance += length;
            }
        }


        public override void Update()
        {
            base.Update();

            if (_bullet.removeFromLevel)
            {
                Level.Remove(this);
                return;
            }

            _timer -= 0.15f;
        }

        public override void Draw()
        {
            Draw(_bullet.DrawEnd, _targetPosition, _bullet.depth.value);
        }
    }
}
