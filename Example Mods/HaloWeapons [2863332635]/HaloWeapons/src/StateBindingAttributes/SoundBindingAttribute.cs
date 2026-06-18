namespace DuckGame.HaloWeapons
{
    public class SoundBindingAttribute : BindingAttribute
    {
        public SoundBindingAttribute(GhostPriority priority) : base(priority, 2, false, false, false)
        {

        }

        public override StateBinding CreateStateBinding()
        {
            return new NetSoundBinding(Priority, MemberName);
        }
    }
}
