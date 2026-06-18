using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    class redEyeEye : Thing
    {
        private SpriteMap _sprite;
        private bool shoot = true;
        public float _rotAngle;
        public override float angle
        {
            get
            {
                return base.angle + Maths.DegToRad(-this._rotAngle);
            }
            set
            {
                this._angle = value;
            }
        }
        protected Vec2 barrel = new Vec2();
        int leftOrRight = Rando.Int(0, 1);
        public redEyeEye(float xpos, float ypos)
          : base(xpos, ypos, (Sprite)null)
        {
            this._sprite = new SpriteMap(GetPath("redEyeEye"), 11, 11, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(5.5f, 5.5f);
            this._collisionSize = new Vec2(11f, 11f);
            this._collisionOffset = new Vec2(-5.5f, -5.5f);
            this._sprite.AddAnimation("aim", 0.11f, true, 1, 1, 1, 1, 2, 2, 3, 3);
            //this._sprite.AddAnimation("aim", 0.09f,true, 1, 1, 1, 1, 2, 2, 3, 3);
            this._sprite.SetAnimation("aim");
            this.barrel = new Vec2(-5f, 0f);
        }
        public override void Update()
        {
            if (this._sprite.frame == 1)
                shoot = true;
            if (this._sprite.frame == 7 && shoot == true)
            {
                Level.Add((Thing)SmallSmoke.New(this.x + Rando.Float(-0.1f, 0.1f), this.y + Rando.Float(-0.1f, 0.1f)));
                SFX.Play("laserRifle", 0.6f, Rando.Float(0.2f) - 0.4f, 0.0f, false);
                shoot = false;

                //Lasery
                ATblueLaser atBlueLaser = new ATblueLaser();
                atBlueLaser.range = 300f + Rando.Float(5f);
                atBlueLaser.penetration = 2f;
                Vec2 vec2 = new Vec2((float)Math.Cos((double)Maths.DegToRad(_rotAngle)), (float)Math.Sin((double)Maths.DegToRad(_rotAngle)));
                blueLaserBullet laserBullet = new blueLaserBullet(x + vec2.x * 8f, y - vec2.y * 8f, (AmmoType)atBlueLaser, _rotAngle - 3f, (Thing)null, false, -1f, false, true);
                laserBullet.firedFrom = this;
                Level.Add((Thing)laserBullet);
                laserBullet = new blueLaserBullet(x + vec2.x * 8f, y - vec2.y * 8f, (AmmoType)atBlueLaser, _rotAngle + 3f, (Thing)null, false, -1f, false, true);
                laserBullet.firedFrom = this;
                Level.Add((Thing)laserBullet);

                //Quad Bloczki
                /*Vec2 travel = Maths.AngleToVec(Maths.DegToRad(this._rotAngle));
                Vec2 vec2 = this.position - travel.normalized * 5f;
                Level.Add((Thing)new QuadLaserBullet(this.x, this.y, travel));
                */

                //RAKIETY
                /*ATMissile atBlueLaser = new ATMissile();
                atBlueLaser.range = 200f + Rando.Float(5f);
                Vec2 vec2 = new Vec2((float)Math.Cos((double)Maths.DegToRad(_rotAngle)), (float)Math.Sin((double)Maths.DegToRad(_rotAngle)));
                Bullet laserBullet = new Bullet(x + vec2.x * 8f, y - vec2.y * 8f, (AmmoType)atBlueLaser, _rotAngle, (Thing)null, false, -1f, false, true);
                laserBullet.firedFrom = this;
                Level.Add((Thing)laserBullet);*/
            }

            if(leftOrRight == 1)
            this._rotAngle += 1f;
            else
            this._rotAngle -= 1f;
            base.Update();
        }
        public override void Draw()
        {
            base.Draw();
            this.depth += 1;
        }
    }
}
