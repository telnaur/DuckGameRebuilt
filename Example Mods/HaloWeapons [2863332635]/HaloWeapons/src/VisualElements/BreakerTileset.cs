namespace DuckGame.HaloWeapons
{
    public class BreakerTileset : HaloTileset
    {
        public BreakerTileset(float x, float y) : base(x, y, Paths.GetSpritePath("breakerTileset.png")) 
        {
            physicsMaterial = PhysicsMaterial.Metal;
            _editorName = "Breaker";
        }
    }
}
