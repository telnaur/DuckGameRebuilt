namespace DuckGame.HaloWeapons
{
    public abstract class BurstingWeapon : HaloWeapon
    {
        private int _bulletsFired;
        private int _burstWait;

        public BurstingWeapon(float x, float y) : base(x, y)
        {
            _fireWait = 0f;
        }

        protected bool CanStartBursting => !Bursting && _wait <= 0f;

        protected bool Bursting { get; set; }

        protected int MaxBulletsFired { get; set; } = 5;

        protected int BurstDelay { get; set; } = 15;

        protected float FireDelay { get; set; } = 1f;

        public override void Update()
        {
            base.Update();

            if (!isServerForObject)
                return;

            if (Bursting)
            {
                if (_burstWait > 0)
                {
                    _burstWait--;
                }
                else
                {
                    DoFireSynchronized();

                    _bulletsFired++;

                    if (_bulletsFired < MaxBulletsFired)
                    {
                        _burstWait = BurstDelay;
                    }
                    else
                    {
                        Bursting = false;
                        _bulletsFired = 0;
                        _wait = FireDelay;
                    }
                }

                return;
            }
        }

        public override void PressAction()
        {
            if (isServerForObject)
                base.PressAction();
            else
                Fire();
        }

        public override void OnPressAction()
        {
            StartBursting();
        }

        protected void StartBursting()
        {
            if (!CanStartBursting)
                return;

            Bursting = true;
        }
    }
}
