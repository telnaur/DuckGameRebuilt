namespace DuckGame.HaloWeapons
{
    public class FloatBindingAttribute : BindingAttribute
    {
        private readonly float _range;

        public FloatBindingAttribute(GhostPriority priority, float range, int bits, bool rotation, bool lerp) : base(priority, bits, rotation, false, lerp)
        {
            _range = range;
        }

        public FloatBindingAttribute(float range = 1f, int bits = 16, bool rotation = false, bool lerp = false) : base(bits, rotation, false, lerp)
        {
            _range = range;
        }

        public override StateBinding CreateStateBinding()
        {
            return new CompressedFloatBinding(Priority, MemberName, _range, Bits, Rotation, Lerp);
        }
    }
}
