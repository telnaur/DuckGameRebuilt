namespace DuckGame.HaloWeapons
{
    public class NMSpikeGrenadeStick : NMEvent
    {
        public SpikeGrenade Grenade;
        public MaterialThing StickTo;

        public NMSpikeGrenadeStick(SpikeGrenade grenade, MaterialThing stickTo)
        {
            Grenade = grenade;
            StickTo = stickTo;
        }

        public NMSpikeGrenadeStick()
        {

        }

        public override void Activate()
        {
            Grenade.Stick(StickTo);
        }
    }
}
