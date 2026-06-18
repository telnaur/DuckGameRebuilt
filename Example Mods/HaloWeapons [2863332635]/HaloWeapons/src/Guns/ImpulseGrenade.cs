using System;
using System.Linq;

namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Guns)]
    public class ImpulseGrenade : HaloGrenade
    {
        private readonly float _explodeRadius = 90f;

        private readonly Color[] _impulseColors = new Color[]
        {
            new Color(24, 44, 82),

            new Color(49, 76, 131),

            new Color(82, 125, 213),

            new Color(82, 149, 246),

            new Color(106, 165, 238)
        };

        public ImpulseGrenade(float x, float y) : base(x, y)
        {
            Timer = 1.1f;
            ActivateSound = Paths.GetSoundPath("impulseGrenadeActivate.wav");
            ExplosionSound = Paths.GetSoundPath("impulseGrenadeExplosion.wav");

            ammo = 1;
            bouncy = 0.4f;
            friction = 0.05f;
        }

        protected override void Activate()
        {
            base.Activate();

            SpriteMap.frame = 1;
        }

        protected override void Explode()
        {
            for (int i = 0; i < Rando.Int(3, 5); i++)
            {
                Color color = _impulseColors[Rando.Int(_impulseColors.Length - 1)];

                float radius = Rando.Float(_explodeRadius / 10f);
                float radiusIncrease = Rando.Float(2f, 8f);
                float width = Rando.Float(_explodeRadius / 60f, _explodeRadius / 35f);

                int lifeDuration = Rando.Int(10, 15);

                Level.Add(new FadingCircle(position, color, radius, radiusIncrease, width, lifeDuration));
            }

            foreach (PhysicsObject thing in Level.CheckCircleAll<PhysicsObject>(position, _explodeRadius).Where(thing => thing != this))
            {
                Vec2 thingPosition = thing.position;

                float angle = Maths.PointDirectionRad(position, thingPosition);

                float cos = (float)Math.Cos(angle);
                float sin = (float)Math.Sin(angle);

                float distance = Vec2.Distance(position, thingPosition);

                thing.velocity += new Vec2(cos, -sin) * _explodeRadius * (distance / _explodeRadius / 2f);
            }

            base.Explode();
        }

        protected override Sprite CreateGraphics(string spritePath)
        {
            return new SpriteMap(spritePath, 9, 9);
        }
    }
}
