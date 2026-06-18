using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    internal class ATOra : Thing
    {
        public StateBinding _theDuckStateBinding = new StateBinding("_theDuck");
        public StateBinding _countStateBinding = new StateBinding("_count");
        public StateBinding _offDirStateBinding = new StateBinding("offDir");
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        public Duck _theDuck;
        public int _count;

        private SpriteMap sprite;

        public ATOra(float xpos, float ypos, sbyte oD, Duck d)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Oraa"), 18, 12);
            graphic = sprite;
            offDir = oD;
            _theDuck = d;
            center = new Vec2(9f, 6f);
            _collisionOffset = new Vec2(-9f, -6f);
            _collisionSize = new Vec2(18f, 12f);
        }

        public override void Update()
        {
            if (isServerForObject && (x > Level.current.bottomRight.x + 200 || x < Level.current.topLeft.x - 200 || _count >= 20))
                Level.Remove(this);

            _count++;

            sprite.flipH = offDir > 0 ? false : true;
            x += offDir * 4f;

            if (alpha >= 0.6f)
                foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(position, 6f))
                {
                    if (materialThing is Duck)
                    {
                        ((Duck)materialThing).Kill((DestroyType)new DTImpale((Thing)materialThing));
                        Level.Remove((this));
                    }
                    if (materialThing is Crate || materialThing is LavaBarrel || materialThing is BlueBarrel || materialThing is YellowBarrel || materialThing is DeathCrate)
                    {
                        Level.Remove((materialThing));
                        Level.Remove((this));
                        ExplosionPart exp = new ExplosionPart(this.position.x, this.position.y);
                        Level.Add((Thing)exp);
                        SFX.Play(GetPath("sounds/thunder"));
                    }
                    if (materialThing is Bullet)
                    {
                        Level.Remove((materialThing));
                        Level.Remove((this));
                    }
                    if (materialThing is Door)
                    {
                        Level.Remove((materialThing));
                        Level.Remove((this));
                        ExplosionPart exp = new ExplosionPart(this.position.x, this.position.y);
                        Level.Add((Thing)exp);
                        SFX.Play(GetPath("sounds/thunder"));
                    }
                    if (materialThing is Block)
                    {
                        Level.Remove((this));
                    }
                }

            base.Update();
        }
    }
}


