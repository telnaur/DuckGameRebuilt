namespace DuckGame.HaloWeapons
{
    public sealed class FadingCircle : Thing, IFadingThing
    {
        private readonly Color _color;
        private readonly float _radiusIncrease;
        private readonly float _width;
        private readonly int _lifeDuration;

        private float _radius;
        private int _timer;

        public FadingCircle(Vec2 position, Color color, float radius, float radiusIncrease, float width, int lifeDuration) : base(position.x, position.y)
        {
            _color = color;
            _radiusIncrease = radiusIncrease;   
            _width = width;
            _lifeDuration = lifeDuration;

            _radius = radius;
            _timer = _lifeDuration;
        }

        public float Timer => _timer;
        public float MaxTime => _lifeDuration;

        public override void Draw()
        {
            _radius += _radiusIncrease;

            Graphics.DrawCircle(position, _radius, _color * alpha, _width, 3f, 500);

            _timer--;
        }
    }
}
