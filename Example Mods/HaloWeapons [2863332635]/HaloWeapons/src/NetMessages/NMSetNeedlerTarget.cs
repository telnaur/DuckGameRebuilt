namespace DuckGame.HaloWeapons
{
    public class NMSetNeedlerTarget : NMEvent
    { 
        public Needler Needler;
        public Duck Target;

        public NMSetNeedlerTarget(Needler needler, Duck target)
        {
            Needler = needler;
            Target = target;
        }

        public NMSetNeedlerTarget()
        {
             
        }

        public override void Activate()
        {
            if (Needler is null)
                return;

            Needler.SetTarget(Target);
        }
    }
}
