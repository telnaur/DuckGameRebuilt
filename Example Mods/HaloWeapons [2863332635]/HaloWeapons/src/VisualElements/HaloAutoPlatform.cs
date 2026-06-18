namespace DuckGame.HaloWeapons
{
    public abstract class HaloAutoPlatform : AutoPlatform
    {
        public HaloAutoPlatform(float x, float y, string spritePath) : base(x, y, spritePath)
        {
            verticalWidth = 14f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;

            _collideBottom = true;
        }
    }
}
