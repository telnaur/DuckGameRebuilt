using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{

    public class eyeFlower : Thing
    {
        private SpriteMap _sprite;
        //protected SpriteMap eyeSprite;
        private bool klatka = false;
        /*public float _rotAngle;
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
        */
        //protected Vec2 barrel = new Vec2();
        //protected Vec2 duckie = new Vec2();
        //private float Timer = 10f;

        public eyeFlower(float xpos, float ypos)
          : base(xpos, ypos, (Sprite)null)
        {
            this._sprite = new SpriteMap(GetPath("eyeFlower"), 10, 28, false);
            this._sprite.AddAnimation("growing", 0.03f, 0 != 0, 1, 2, 3, 4, 5, 6, 7, 8);
            this._sprite.AddAnimation("grown", 0.8f, 0 != 0, 1, 2, 2, 3, 3);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(5f, 26f);
            this._collisionSize = new Vec2(10f, 26f);
            this._collisionOffset = new Vec2(-5f, -26f);
            this._sprite.SetAnimation("growing");
            //this.eyeSprite = new SpriteMap(GetPath("eyeEye"), 11, 11, false);
            //this.eyeSprite.center = new Vec2(5.5f, 5.5f);
            //this.barrel = new Vec2(6f, -26f);
        }

        public override void Update()
        {
            if (this._sprite.frame == 1 && klatka == false)
            {
                SFX.Play("littleSplash", 0.6f, Rando.Float(0.2f) - 0.4f, 0.0f, false);
                Level.Add((Thing)SmallSmoke.New(this.x, this.y - 4f));
                klatka = true;
            }
            if (this._sprite.frame == 2 && klatka == true)
            {
                SFX.Play("littleSplash", 0.6f, Rando.Float(0.2f) - 0.4f, 0.0f, false);
                Level.Add((Thing)SmallSmoke.New(this.x, this.y - 8f));
                klatka = false;
            }
            if (this._sprite.frame == 3 && klatka == false)
            {
                SFX.Play("littleSplash", 0.6f, Rando.Float(0.2f) - 0.4f, 0.0f, false);
                Level.Add((Thing)SmallSmoke.New(this.x, this.y - 12f));
                klatka = true;
            }
            if (this._sprite.frame == 4 && klatka == true)
            {
                SFX.Play("littleSplash", 0.6f, Rando.Float(0.2f) - 0.4f, 0.0f, false);
                Level.Add((Thing)SmallSmoke.New(this.x, this.y - 16f));
                klatka = false;
            }
            if (this._sprite.frame == 5 && klatka == false)
            {
                SFX.Play("littleSplash", 0.6f, Rando.Float(0.2f) - 0.4f, 0.0f, false);
                Level.Add((Thing)SmallSmoke.New(this.x, this.y - 18f));
                klatka = true;
            }
            if (this._sprite.frame == 6 && klatka == true)
            {
                SFX.Play("littleSplash", 0.6f, Rando.Float(0.2f) - 0.4f, 0.0f, false);
                Level.Add((Thing)SmallSmoke.New(this.x, this.y - 20f));
                klatka = false;
            }
            if (this._sprite.frame == 7 && klatka == false)
            {
                SFX.Play("swallow", 0.6f, Rando.Float(0.2f) - 0.4f, 0.0f, false);
                Level.Add((Thing)SmallSmoke.New(this.x, this.y - 20f));
                klatka = true;
                if (this.isServerForObject)
                {
                    Level.Add((Thing)SmallSmoke.New(this.x + 0.5f, this.y - 21.5f));
                    eyeEye eyeEye = new eyeEye(this.x + 0.5f, this.y -21.5f);
                    Level.Add((Thing)eyeEye);
                }
                //this.eyeSprite.frame = 1;
            }
            /*
            if (this._sprite.frame == 7) {
                Timer -= 0.04f;
                if (Timer > 4f)
                    this.eyeSprite.frame = 1;
                if (Timer >= 1.5f && Timer <= 4f)
                    this.eyeSprite.frame = 2;
                if (Timer > 0f && Timer < 1.5f)
                    this.eyeSprite.frame = 3;
                if (Timer <= 0f)
                {
                    this.eyeSprite.frame = 3;
                    Level.Add((Thing)SmallSmoke.New(this.x, this.y - 20f));
                    //SHOOT
                    Vec2 vec2 = this.Offset(this.barrel);
                    Net net = new Net(vec2.x, vec2.y, null);
                    this.Fondle((Thing)net);
                    net.hSpeed = duckie.x * 6f;
                    net.vSpeed = duckie.y * 6f;
                    Level.Add((Thing)net);
                    Timer = 8f;
                }
                //this.eyeSprite.angle += 10f;
                foreach (Duck duck in Level.current.things[typeof(Duck)])
                {
                    if ((Level.CheckLine<Block>(this.position, duck.position, (Thing)null) == null)){
                        duckie = duck.position;
                    }
                }
            }
            */
                    base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            /*
            this.eyeSprite.alpha = this.graphic.alpha;
            this.eyeSprite.depth = this.depth + 1;
            this.Draw((Sprite)this.eyeSprite, new Vec2(0.5f, -21.5f), 1);
            */
        }
    }

}