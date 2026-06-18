namespace DuckGame.HaloWeapons
{
    public class SpikeGrenadePin : EjectedShell
    {
        public SpikeGrenadePin(float x, float y) : base(x, y, Paths.GetSpritePath("spikeGrenadePin.png"), "metalBounce")
        {
            Utilities.SetCollisionBox(this, 3f, 4f);
        }
    }
}
