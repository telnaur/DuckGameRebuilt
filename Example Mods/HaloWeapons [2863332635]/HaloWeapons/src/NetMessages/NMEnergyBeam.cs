namespace DuckGame.HaloWeapons
{
    public class NMEnergyBeam : NMEvent
    {
        public Vec2 Position;
        public float Angle;

        public NMEnergyBeam(Vec2 position, float angle)
        {
            Position = position;
            Angle = angle;
        }

        public NMEnergyBeam()
        {

        }

        public override void Activate()
        {
            Level.Add(new EnergyBeam(Position.x, Position.y)
            {
                angle = Angle,
                isLocal = false
            });
        }
    }
}
