namespace DuckGame.HaloWeapons
{
    public class CindershotBullet : Bullet
    {
        public CindershotBullet(float x, float y, AmmoType type, float angle, Thing owner, bool rebound, float distance, bool tracer, bool network) : base(x, y, type, angle, owner, rebound, distance, tracer, network)
        {

        }

        protected override void Rebound(Vec2 position, float direction, float rng)
        {
            CindershotBullet bullet = ammo.GetBullet(position.x, position.y, null, -direction, firedFrom, rng, _tracer, true) as CindershotBullet;

            bullet.isLocal = isLocal;
            bullet.lastReboundSource = lastReboundSource;
            bullet.connection = connection;
            reboundCalled = true;

            Level.Add(bullet);
        }
    }
}
