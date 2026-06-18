namespace DuckGame.HaloWeapons
{
    public class DropWallParticle : FadingParticle
    {
        public DropWallParticle(float x, float y) : base(x, y)
        {
            _bounceEfficiency = 0.6f;
        }

        public override void Draw()
        {
            Graphics.DrawRect(position, position + new Vec2(1f), Color * alpha, depth);
        }
    }
}
