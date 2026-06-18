namespace DuckGame.HaloWeapons
{
    [NoSkins]
    public abstract class HaloGrenade : HaloWeapon
    {
        private float _levelRemoveTimer = 0.04f;

        public HaloGrenade(float x, float y) : base(x, y)
        {
            ammo = 1;
        }

        [Binding] protected bool Activated { get; set; }
        [Binding] protected float Timer { get; set; } = 1.2f;

        protected string ActivateSound { get; set; }
        protected string ExplosionSound { get; set; }
        protected bool Exploded { get; set; }

        public override void Update()
        {
            base.Update();

            if (Activated)
            {
                if (Exploded)
                {
                    if (isServerForObject)
                    {
                        _levelRemoveTimer -= 0.01f;

                        if (_levelRemoveTimer <= 0f)
                            Level.Remove(this);
                    }
                }
                else
                {
                    Timer -= 0.01f;

                    if (Timer <= 0f)
                        Explode();
                }
            }
        }

        public override void OnPressAction()
        {
            if (Activated)
                return;

            Activate();
        }

        protected virtual void Activate()
        {
            SFX.PlaySynchronized(ActivateSound);

            if (isServerForObject)
                Activated = true;
        }

        protected virtual void Explode()
        {
            SFX.Play(ExplosionSound);

            Exploded = true;
        }
    }
}
