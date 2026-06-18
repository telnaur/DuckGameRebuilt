namespace DuckGame.HaloWeapons
{
    public class NMEnergySwordNoAmmo : NMEvent
    {
        public EnergySword Sword;

        public NMEnergySwordNoAmmo(EnergySword sword)
        {
            Sword = sword;
        }

        public NMEnergySwordNoAmmo()
        {

        }

        public override void Activate()
        {
            Sword.OnRanOutOfAmmo();
        }
    }
}
