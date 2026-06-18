namespace DuckGame.HaloWeapons
{
    public class Vec2BindingAttribute : BindingAttribute
    {
        private readonly int _range;

        public Vec2BindingAttribute(GhostPriority priority, int range, bool velocity, bool lerp) : base(priority, -1, false, velocity, lerp)
        {
            _range = range;
        }

        public Vec2BindingAttribute(int range = int.MaxValue, bool velocity = false, bool lerp = false) : base(-1, false, velocity, lerp)
        {
            _range = range;
        }

        public override StateBinding CreateStateBinding()
        {
            return new CompressedVec2Binding(Priority, MemberName, _range, Velocity, Lerp);
        }
    }
}
