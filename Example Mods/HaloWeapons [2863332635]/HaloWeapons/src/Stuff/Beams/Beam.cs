using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DuckGame.HaloWeapons
{
    public abstract class Beam : Thing
    {
        private readonly HashSet<MaterialThing> _ignore = new HashSet<MaterialThing>();

        private IEnumerable<MaterialThing> _currentlyImpacting;
        private Vec2 _travelEnd;

        public Beam(float x, float y) : base(x, y)
        {
            depth = 0.6f;
        }

        protected Vec2 TravelEnd => _travelEnd;
        protected float CurrentLength => Vec2.Distance(position, TravelEnd);
        protected IEnumerable<MaterialThing> CurrentImpacting => _currentlyImpacting;

        protected Texture2D Texture { get; set; }
        protected float Thickness { get; set; } = 2f;
        protected float Range { get; set; } = 10f;
        protected float Penetration { get; set; } = 1f;

        public override void Update()
        {
            var tracer = new ATTracer()
            {
                range = Range,
                penetration = Penetration,
                bulletThickness = Thickness,
            };

            var bullet = new Bullet(x, y, tracer, angleDegrees, tracer: true);

            Level.Add(bullet);

            _travelEnd = bullet.end;
            _currentlyImpacting = bullet._currentlyImpacting;

            if (isLocal && isServerForObject)
            {
                foreach (MaterialThing thing in _currentlyImpacting)
                {
                    if (_ignore.Contains(thing))
                        continue;

                    SuperFondle(thing, DuckNetwork.localConnection);
                    thing.Destroy(new DTIncinerate(this));
                }
            }
        }

        public override void Draw()
        {
            if (Texture is null)
                return;

            Graphics.DrawTexturedLine(Texture, position, _travelEnd, Color.White * alpha, Thickness, depth);
        }

        public override void Added(Level parent, bool redoLayer, bool reInit)
        {
            base.Added(parent);

            Update();
        }

        public void AddIgnoredThing(MaterialThing thing)
        {
            _ignore.Add(thing);
        }
    }
}
