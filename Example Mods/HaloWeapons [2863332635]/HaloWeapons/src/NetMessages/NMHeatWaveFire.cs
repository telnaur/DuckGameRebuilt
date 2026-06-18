namespace DuckGame.HaloWeapons
{
    public class NMHeatWaveFire : NMEvent
    {
        public HeatWave HeatWave;
        public Vec2 Position;
        public float BarrelAngle;
        public sbyte Direction;
        public sbyte BulletModeIndex;

        public NMHeatWaveFire(HeatWave heatWave, Vec2 position, float barrelAngle, sbyte direction, sbyte bulletModeIndex)
        {
            HeatWave = heatWave;
            Position = position;
            BarrelAngle = barrelAngle;
            Direction = direction;
            BulletModeIndex = bulletModeIndex;
        }

        public NMHeatWaveFire()
        {

        }

        public override void Activate()
        {
            HeatWave.OnFireMessage(Position, BarrelAngle, Direction, BulletModeIndex);
        }
    }
}
