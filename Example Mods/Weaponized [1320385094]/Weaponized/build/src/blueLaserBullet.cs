using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class blueLaserBullet : Bullet
    {
        private Tex2D _beem;
        private float _thickness;

        public blueLaserBullet(float xval, float yval, AmmoType type, float ang = -1f, Thing owner = null, bool rbound = false, float distance = -1f, bool tracer = false, bool network = false)
          : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
            this._thickness = type.bulletThickness;
            this._beem = Content.Load<Tex2D>(GetPath("redLaserBeam"));
        }

        public override void Draw()
        {
            if (this._tracer || (double)this._bulletDistance <= 0.100000001490116)
                return;
            float length = (this.drawStart - this.drawEnd).length;
            float val = 0.0f;
            float num1 = (float)(1.0 / ((double)length / 8.0));
            float num2 = 0.0f;
            float num3 = 8f;
            while (true)
            {
                bool flag = false;
                if ((double)val + (double)num3 > (double)length)
                {
                    num3 = length - Maths.Clamp(val, 0.0f, 99f);
                    flag = true;
                }
                num2 += num1;
                DuckGame.Graphics.DrawTexturedLine((Tex2D)this._beem, this.drawStart + this.travelDirNormalized * val, this.drawStart + this.travelDirNormalized * (val + num3), Color.White * num2, this._thickness, (Depth)0.6f);
                if (!flag)
                    val += 8f;
                else
                    break;
            }
        }

        protected override void Rebound(Vec2 pos, float dir, float rng)
        {
            Bullet.isRebound = true;
            blueLaserBullet bluelaserBullet = new blueLaserBullet(pos.x, pos.y, this.ammo, dir, (Thing)null, this.rebound, rng, false, false);
            Bullet.isRebound = false;
            bluelaserBullet._teleporter = this._teleporter;
            bluelaserBullet.firedFrom = this.firedFrom;
            Level.current.AddThing((Thing)bluelaserBullet);
            Level.current.AddThing((Thing)new LaserRebound(pos.x, pos.y));
        }
    }
}