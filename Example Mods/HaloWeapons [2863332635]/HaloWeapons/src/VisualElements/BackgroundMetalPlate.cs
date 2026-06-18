namespace DuckGame.HaloWeapons
{
    public class BackgroundMetalPlate : HaloBackgroundTile
	{
        public BackgroundMetalPlate(float x, float y) : base(x, y)   
        {
            graphic = Resources.LoadSpriteMap("backgroundMetalPlate.png", 16, 16);
            _editorName = "MetalPlate";
        }
    }
}
