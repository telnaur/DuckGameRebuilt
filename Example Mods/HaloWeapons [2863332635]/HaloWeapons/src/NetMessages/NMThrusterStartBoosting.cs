namespace DuckGame.HaloWeapons
{
    public class NMThrusterStartBoosting : NMEvent
    {
        public Thruster Thruster;

        public NMThrusterStartBoosting(Thruster thruster)
        {
            Thruster = thruster;
        }

        public NMThrusterStartBoosting()
        {

        }

        public override void Activate()
        {
            Thruster.StartBoosting();
        }
    }
}
