using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Environment")]
    public class Cornfield : MaterialThing
    {
        public StateBinding _burnLifeBinding = new StateBinding("_burnLife", -1, false, false);
        public float _burnLife = 1f;
        public float _burnWait;
        public bool burntOut;

        public Cornfield(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite(GetPath("Cornfield"));
            this.center = new Vec2(24f, 24f);
            this._collisionSize = new Vec2(40f, 44f);
            this._collisionOffset = new Vec2(-20f, -20f);
            this.depth = (Depth)0.9f;
            this.hugWalls = WallHug.Left | WallHug.Right | WallHug.Floor;
            this.flammable = 0.6f;
            this._editorName = "Cornfield";
            this.editorTooltip = "Ducks love hiding in these. Watch these hooligans burn.";
        }

        public override void Draw()
        {
            this.graphic.flipH = (int)this.offDir <= 0;
            base.Draw();
        }

        public override void UpdateOnFire()
        {
            if (!this.onFire)
                return;
            this._burnWait -= 0.01f;
            if ((double)this._burnWait < 0.0)
            {
                Level.Add((Thing)SmallFire.New(10f, 20f, 0.0f, 0.0f, false, (MaterialThing)this, false, (Thing)this, false));
                Level.Add((Thing)SmallFire.New(0f, 15f, 0.0f, 0.0f, false, (MaterialThing)this, false, (Thing)this, false));
                Level.Add((Thing)SmallFire.New(-10f, 20f, 0.0f, 0.0f, false, (MaterialThing)this, false, (Thing)this, false));
                for (int index = 0; index < 2; ++index)
                    Level.Add((Thing)SmallFire.New(this.x - 6f + Rando.Float(12f), this.y - 8f + Rando.Float(4f), Rando.Float(4f) - 3f, 1f - Rando.Float(2.5f), false, (MaterialThing)null, true, (Thing)this, false));
                this._burnWait = 1f;
            }
            if ((double)this.burnt >= 1.0)
                return;
            this.burnt += (float)(3.0 / 1000.0);
        }

        public override void Update()
        {
            if (!this.burntOut && (double)this.burnt >= 1.0)
            {
                this.graphic = new Sprite(GetPath("CornfieldBurn"));
                Vec2 vec2 = this.Offset(new Vec2(10f, -2f));
                Level.Add((Thing)SmallSmoke.New(vec2.x, vec2.y));
                Vec2 vec3 = this.Offset(new Vec2(0f, 0.0f));
                Level.Add((Thing)SmallSmoke.New(vec3.x, vec3.y));
                Vec2 vec4 = this.Offset(new Vec2(-10f, 3f));
                Level.Add((Thing)SmallSmoke.New(vec4.x, vec4.y));
                Vec2 vec5 = this.Offset(new Vec2(6f, -12f));
                Level.Add((Thing)SmallSmoke.New(vec5.x, vec5.y));
                Vec2 vec6 = this.Offset(new Vec2(-8f, -10f));
                Level.Add((Thing)SmallSmoke.New(vec6.x, vec6.y));
                this._onFire = false;
                this.flammable = 0.0f;
                this.burntOut = true;
            }
            base.Update();
        }

    }
}
