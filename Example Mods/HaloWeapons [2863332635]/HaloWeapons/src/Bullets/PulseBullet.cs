using System.Collections.Generic;
using System.Linq;

namespace DuckGame.HaloWeapons
{
    public class PulseBullet : Bullet
    {
        private readonly List<Duck> _targets = new List<Duck>();
        private readonly float _lightingRadius = 14f;

        public PulseBullet(float x, float y, AmmoType type, float angle, Thing owner, bool rebound, float distance, bool tracer, bool network) : base(x, y, type, angle, owner, rebound, distance, tracer, network)
        {

        }

        public Vec2 DrawEnd => drawEnd;

        public static IEnumerable<PulseBullet> CheckCircle(Vec2 position, float radius)
        {
            foreach (PulseBullet bullet in Level.current.things.GetDynamicObjects(typeof(PulseBullet)))
            {
                if (bullet.removeFromLevel)
                    continue;

                Vec2 closestCorner;

                Vec2 bulletPosition = bullet.DrawEnd;
                float bulletX = bulletPosition.x;
                float bulletY = bulletPosition.y;

                float halfWidth = bullet.width / 2f;
                float halfHeight = bullet.height / 2f;

                float x = position.x;
                float y = position.y;

                closestCorner.x = x > bulletX ? bulletX + halfWidth : bulletX - halfWidth;
                closestCorner.y = y > bulletY ? bulletY + halfHeight : bulletY - halfHeight;

                float distance = Vec2.Distance(position, closestCorner);

                if (distance <= radius)
                    yield return bullet;
            }
        }

        public override void Update()
        {
            base.Update();

            if (!_initializedDraw)
                return;

            foreach (Duck duck in Level.CheckCircleAll<Duck>(drawEnd, _lightingRadius).OrderBy(duck => Distance(duck)))
            {
                if (duck == owner || _targets.Contains(duck))
                    continue;

                Vec2 targetPosition = duck.position;

                if (!LineIsClear(drawEnd, targetPosition))
                    continue;

                Level.Add(new PulseBulletLighting(this, targetPosition));

                DevConsole.Log(targetPosition.ToString());

                if (isLocal)
                    duck.Destroy(new DTIncinerate(this));

                _targets.Add(duck);
            }
        }

        public override void Draw()
        {
            if (!_initializedDraw)
                return;

            Vec2 position = drawEnd;

            Sprite sprite = ammo.sprite;
            sprite.angle = -Maths.PointDirectionRad(Vec2.Zero, travelDirNormalized);
            Graphics.Draw(sprite, position.x, position.y);

            foreach (PulseBullet bullet in CheckCircle(drawEnd, 50f).OrderBy(bullet => Vec2.Distance(drawEnd, bullet.DrawEnd)))
            {
                if (bullet == this)
                    continue;

                Vec2 bulletPosition = bullet.DrawEnd;
                float bulletX = bulletPosition.x;

                float x = drawEnd.x;
                float speed = velocity.x;

                if (speed * bullet.velocity.x < 0f)
                    continue;

                if (!LineIsClear(drawEnd, bulletPosition))
                    continue;

                if (bulletX > x)
                    continue;

                PulseBulletLighting.Draw(drawEnd, bulletPosition, depth.value);

                break;
            }
        }

        private bool LineIsClear(Vec2 start, Vec2 end)
        {
            foreach (MaterialThing thing in Level.CheckLineAll<MaterialThing>(start, end))
                if (thing.thickness > 0.5f)
                    return false;

            return true;
        }
    }
}
