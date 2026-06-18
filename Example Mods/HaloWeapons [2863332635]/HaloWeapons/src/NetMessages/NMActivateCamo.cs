namespace DuckGame.HaloWeapons
{
    public class NMActivateCamo : NMEvent
    {
        public ActiveCamo ActiveCamo;

        public NMActivateCamo(ActiveCamo activeCamo)
        {
            ActiveCamo = activeCamo;
        }

        public NMActivateCamo()
        {

        }

        public override void Activate()
        {
            ActiveCamo.Activate();
        }
    }
}
