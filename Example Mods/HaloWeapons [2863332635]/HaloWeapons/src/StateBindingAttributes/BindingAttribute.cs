using System;

namespace DuckGame.HaloWeapons
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BindingAttribute : Attribute
    {
        public BindingAttribute(GhostPriority priority, int bits, bool rotation, bool velocity, bool lerp)
        {
            Priority = priority;
            Bits = bits;
            Rotation = rotation;
            Velocity = velocity;
            Lerp = lerp;
        }

        public BindingAttribute(int bits = -1, bool rotation = false, bool velocity = false, bool lerp = false)
        {
            Bits = bits;
            Rotation = rotation;
            Velocity = velocity;
            Lerp = lerp;
        }

        internal string MemberName { get; set; } = string.Empty;

        protected GhostPriority Priority { get; private init; }
        protected int Bits { get; private init; }
        protected bool Rotation { get; private init; }
        protected bool Velocity { get; private init; }
        protected bool Lerp { get; private init; }

        public virtual StateBinding CreateStateBinding()
        {
            return new StateBinding(Priority, MemberName, Bits, Rotation, Velocity, Lerp);
        }
    }
}
