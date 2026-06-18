namespace DuckGame.HaloWeapons
{
    public class GlassParticle : FadingParticle
    {
        private readonly SpriteMap _sprite = new SpriteMap("windowDebris", 8, 8);

        public GlassParticle(float x, float y) : base(x, y)
        {
            _sprite.frame = Rando.Int(7);
            _sprite.CenterOrigin();

            graphic = _sprite;

            _bounceEfficiency = 0.3f;
        }

        public override Color Color
        {
            get
            {
                return base.Color;
            }

            set
            {
                _sprite.color = value;

                base.Color = value;
            }
        }
    }
}
