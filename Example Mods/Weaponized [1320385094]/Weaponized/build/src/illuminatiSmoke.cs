using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class illuminatiSmoke : Thing
    {
        public Vec2 move = new Vec2();
        public Vec2 fly = new Vec2();
        private float _angleInc;
        private float _scaleInc;
        private float _fade;
        private float _fastGrow;
        private Sprite _backgroundSmoke;

        public illuminatiSmoke(float xpos, float ypos)
          : base(xpos, ypos, (Sprite)null)
        {
            this.xscale = 0.1f + Rando.Float(0.1f);
            this.yscale = this.xscale;
            this.angle = Maths.DegToRad(Rando.Float(360f));
            this._fastGrow = 0.6f + Rando.Float(0.3f);
            this._angleInc = Maths.DegToRad(Rando.Float(20f) - 1f);
            this._scaleInc = 1f / 1000f + Rando.Float(1f / 1000f);
            this._fade = 0.0015f + Rando.Float(1f / 1000f);
            this.move.x = Rando.Float(0.2f) - 0.1f;
            this.move.y = Rando.Float(0.2f) - 0.1f;
            GraphicList graphicList = new GraphicList();
            Sprite graphic1 = new Sprite(GetPath("illuminati"), 0.0f, 0.0f);
            graphic1.depth = (Depth)1f;
            graphic1.CenterOrigin();
            graphicList.Add(graphic1);
            Sprite graphic2 = new Sprite(GetPath("illuminati"), 0.0f, 0.0f);
            graphic2.CenterOrigin();
            graphicList.Add(graphic2);
            this.graphic = (Sprite)graphicList;
            this.center = new Vec2(0.0f, 0.0f);
            this.depth = (Depth)1f;
            this._backgroundSmoke = new Sprite(GetPath("illuminati"), 0.0f, 0.0f);
        }

        public override void Update()
        {
            this.angle += (float)(double)this._angleInc;
            this.xscale += (float)(double)this._scaleInc;
            if ((double)this._fastGrow > 0.0)
            {
                this._fastGrow -= 0.05f;
                this.xscale += 0.03f;
            }
            if ((double)this.fly.x > 0.00999999977648258 || (double)this.fly.x < -0.00999999977648258)
            {
                this.x += (float)(double)this.fly.x;
                this.fly.x *= 0.9f;
            }
            if ((double)this.fly.y > 0.00999999977648258 || (double)this.fly.y < -0.00999999977648258)
            {
                this.y += (float)(double)this.fly.y;
                this.fly.y *= 0.9f;
            }
            this.yscale = this.xscale;
            this.x += (float)(double)this.move.x;
            this.y += (float)(double)this.move.y;
            this.xscale -= 0.005f;
            if ((double)this.xscale >= 0.100000001490116)
                return;
            Level.Remove((Thing)this);
        }
    }
}