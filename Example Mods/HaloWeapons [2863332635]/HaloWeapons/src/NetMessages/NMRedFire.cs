namespace DuckGame.HaloWeapons
{
    public class NMRedFire : NMEvent
    {
        public ushort FireIndex;

        public NMRedFire(SmallFire fire)
        {
            FireIndex = fire.netIndex;
        }

        public NMRedFire()
        {

        }

        public override void Activate()
        {
            RedFire.Add(FireIndex);
        }
    }
}
