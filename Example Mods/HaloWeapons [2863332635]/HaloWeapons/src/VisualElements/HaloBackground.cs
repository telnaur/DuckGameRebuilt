namespace DuckGame.HaloWeapons
{
    public abstract class HaloBackground : BackgroundUpdater
    {
        public HaloBackground(float x, float y) : base(x, y)
        {
            depth = 0.9f;
            layer = Layer.Foreground;

            _visibleInGame = false;

            Utilities.SetCollisionBox(this, new Vec2(16f));
        }

        protected string SpriteName { get; set; }
        protected float DefaultSpeed { get; set; }

        public override void Initialize()
        {
            Level level = Level.current;

            if (level is Editor)
                return;

            level.backgroundColor = backgroundColor;

            ParallaxBackground parallax = CreateParallax();

            if (parallax is not null)
            {
                AddZones(parallax);
                _parallax = parallax;
                Level.Add(_parallax);
            }
        }

        public override void Terminate()
        {
            Level.Remove(_parallax);
        }

        protected abstract void AddZones(ParallaxBackground parallax);

        protected virtual ParallaxBackground CreateParallax()
        {
            return new ParallaxBackground(Paths.GetSpritePath(SpriteName), 0f, 0f, 3);
        }

        protected void AddZonesRange(ParallaxBackground parallax, int from, int to, float distance, float? speed = null, bool moving = false)
        {
            if (parallax is null || to < from)
                return;

            for (int y = from; y < to + 1; y++)
                parallax.AddZone(y, distance, GetSpeedValue(speed), moving);
        }

        protected void AddSprite(ParallaxBackground parallax, Sprite sprite, Vec2 position, int zone, float distance, float? speed = null, bool moving = false)
        {
            sprite.position = position;
            sprite.depth = -depth.value;

            parallax.AddZoneSprite(sprite, zone, distance, GetSpeedValue(speed), moving);
        }

        private float GetSpeedValue(float? speed)
        {
            return speed is null ? DefaultSpeed : speed.Value;
        }
    }
}
