using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    class eyeEye : Thing
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
        public eyeEye(float xpos, float ypos)
          : base(xpos, ypos, (Sprite)null)
        {
            this._sprite = new SpriteMap(GetPath("eyeEye"), 11, 11, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(5.5f, 5.5f);
            this._collisionSize = new Vec2(11f, 11f);
            this._collisionOffset = new Vec2(-5.5f, -5.5f);
            this._sprite.AddAnimation("aim", 0.23f,true, 1, 1, 1, 1, 2, 2, 3, 3);
            this._sprite.SetAnimation("aim");
            this.barrel = new Vec2(5f, 0f);
        }
        public override void Update()
        {
            if (this._sprite.frame == 1)
                shoot = true;
            if (this._sprite.frame == 7 && shoot == true)
            {
                if (this.isServerForObject)
                {
                    Level.Add((Thing)SmallSmoke.New(this.x + Rando.Float(-0.1f, 0.1f), this.y + Rando.Float(-0.1f, 0.1f)));
                    SFX.Play("pistolFire", 0.6f, Rando.Float(0.2f) - 0.4f, 0.0f, false);
                    shoot = false;

                    AT9mm atBlueLaser = new AT9mm();
                    atBlueLaser.range = 90f + Rando.Float(5f);
                    Vec2 vec2 = new Vec2((float)Math.Cos((double)Maths.DegToRad(_rotAngle)), (float)Math.Sin((double)Maths.DegToRad(_rotAngle)));
                    Bullet laserBullet = new Bullet(x + vec2.x * 8f, y - vec2.y * 8f, (AmmoType)atBlueLaser, _rotAngle, (Thing)null, false, -1f, false, true);
                    laserBullet.firedFrom = this;
                    Level.Add((Thing)laserBullet);
                }
            }

            if(leftOrRight == 1)
            this._rotAngle += 1.6f;
            else
            this._rotAngle -= 1.6f;
            base.Update();
        }
        public override void Draw()
        {
            base.Draw();
            this.depth += 1;
        }
    }
}
