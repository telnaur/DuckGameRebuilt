namespace DuckGame.HaloWeapons
{
    public abstract class HaloBackgroundTile : BackgroundTile
    {
        public HaloBackgroundTile(float x, float y) : base(x, y)    
        {
            center = new Vec2(8f, 8f);
            collisionSize = center * 2f;
            collisionOffset = -center;

            _opacityFromGraphic = true;
        }
    }
}
