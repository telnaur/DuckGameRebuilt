namespace DuckGame.HaloWeapons
{
    public abstract class HaloTileset : AutoBlock
    {
        public HaloTileset(float x, float y, string spritePath) : base(x, y, spritePath)
        {
            verticalWidthThick = 15f;
            verticalWidth = 14f;
            horizontalHeight = 15f;
        }
    }
}
