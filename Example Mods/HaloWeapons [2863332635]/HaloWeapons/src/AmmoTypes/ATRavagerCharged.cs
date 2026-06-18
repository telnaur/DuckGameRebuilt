using System.Linq;

namespace DuckGame.HaloWeapons
{
    public class ATRavagerCharged : ATRavager
    {
        public ATRavagerCharged()
        {
            SetSprite(Resources.LoadSpriteMap("ravagerChargedBullet.png", 17, 9));

            range = 700f;
            bulletSpeed = 8f;
            penetration = 0.2f;
            flawlessPipeTravel = true;
        }

        public override void OnHit(bool destroyed, Bullet bullet)
        {
            if (!bullet.isLocal)
                return;

            if (destroyed)
            {
                MaterialThing currentlyImpactig = bullet._currentlyImpacting.FirstOrDefault();

                if (currentlyImpactig is not null)
                    currentlyImpactig.Destroy(new DTImpact(bullet));

                for (int i = 0; i < Rando.Int(8, 10); i++)
                {
                    Vec2 velocity = new Vec2(Rando.Float(-3f, 3f), Rando.Float(-3f, -1f));

                    SmallFire fire = SmallFire.New(bullet.x, bullet.y, velocity.x, velocity.y, firedFrom: bullet.firedFrom);
                    RedFire.Add(fire, true);

                    Level.Add(fire);
                }
            }
        }
    }
}
