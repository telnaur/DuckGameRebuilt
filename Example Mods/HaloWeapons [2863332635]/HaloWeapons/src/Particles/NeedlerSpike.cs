namespace DuckGame.HaloWeapons
{
    public class NeedlerSpike : EjectedShell
    {
        public NeedlerSpike(float x, float y) : base(x, y, Paths.GetSpritePath("needlerSpike.png"), "metalBounce")
        {
            Utilities.SetCollisionBox(this, new Vec2(5f));
        }
    }
}
