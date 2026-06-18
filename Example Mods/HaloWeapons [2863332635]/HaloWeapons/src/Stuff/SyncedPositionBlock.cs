namespace DuckGame.HaloWeapons
{
    public abstract class SyncedPositionBlock : Block
    {
        public readonly StateBinding PositionBinding = new StateBinding("netPosition");

        private sbyte _direction;

        public SyncedPositionBlock(float x, float y) : base(x, y)   
        {

        }

        [Binding]
        public virtual sbyte Direction
        {
            get
            {
                return _direction;
            }

            set
            {
                _direction = value;
            }
        }
    }
}
