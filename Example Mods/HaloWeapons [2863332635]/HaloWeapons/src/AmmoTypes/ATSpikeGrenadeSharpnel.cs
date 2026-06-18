namespace DuckGame.HaloWeapons
{
    public class ATSpikeGrenadeSharpnel : ATReboundBullet
    {
        public ATSpikeGrenadeSharpnel()
        {
            range = 150f;
            bulletSpeed = 7f;
            penetration = 0.4f;
            bulletColor = new Color(147, 75, 6);
            bulletThickness = 0.8f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            Level.Add(new SpikeGrenadePin(x, y)
            {
                hSpeed = Rando.Float(1.5f, 2f) * -dir,
                vSpeed = -Rando.Float(0.5f, 2f)
            });
        }
    }
}
