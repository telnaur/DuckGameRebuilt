namespace DuckGame.HaloWeapons
{
    public class HomingBullet : Bullet
    {
        private readonly float _initialAngle;

        private bool _didAim;

        public HomingBullet(float x, float y, AmmoType type, float angle, Thing owner, bool rebound, float distance, bool tracer, bool network) : base(x, y, type, angle, owner, rebound, distance, tracer, network)
        {
            _initialAngle = Maths.DegToRad(angle);
        }

        public override void Update()
        {
            base.Update();

            IHomingWeapon weapon = FindWeapon();

            if (weapon is null || travelTime < 0.25f || _didAim)
                return;

            _didAim = true;
            Duck target = weapon.Target;

            if (target is null)
                return;

            Vec2 targetPosition = target.GetTruePosition();

            velocity = velocity.Rotate(_initialAngle - Maths.PointDirectionRad(drawEnd, targetPosition), Vec2.Zero);
        } 

        private IHomingWeapon FindWeapon()
        {
            if (_firedFrom is IHomingWeapon weapon)
                return weapon;

            if (owner is Duck duck && duck.holdObject is IHomingWeapon homingWeapon)
                return homingWeapon;

            return null;
        }
    }
}
