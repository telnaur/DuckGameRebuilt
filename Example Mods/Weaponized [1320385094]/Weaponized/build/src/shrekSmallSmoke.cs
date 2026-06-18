using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class shrekSmallSmoke : Thing
    {
        private static int kMaxObjects = 64;
        private static shrekSmallSmoke[] _objects = new shrekSmallSmoke[shrekSmallSmoke.kMaxObjects];
        private static int _lastActiveObject = 0;
        public static bool shortlife = false;
        private float _orbitInc = Rando.Float(5f);
        private float _life = 1f;
        private float _rotSpeed = Rando.Float(0.05f, 0.15f);
        private float _distPulseSpeed = Rando.Float(0.05f, 0.15f);
        private float _distPulse = Rando.Float(5f);
        private float s1 = 1f;
        private float s2 = 1f;
        private float lifeTake = 0.05f;
        private SpriteMap _sprite2;
        private SpriteMap _sprite;
        private SpriteMap _orbiter;

        public SpriteMap sprite
        {
            get
            {
                return this._sprite;
            }
        }

        private shrekSmallSmoke()
          : base(0.0f, 0.0f, (Sprite)null)
        {
            this._sprite = new SpriteMap(GetPath("shrekTinySmokeTestFront"), 16, 16, false);
            int num1 = Rando.Int(3) * 4;
            this._sprite.AddAnimation("idle", 0.1f, 1 != 0, num1);
            this._sprite.AddAnimation("puff", Rando.Float(0.15f, 0.25f), 0 != 0, num1, 1 + num1, 2 + num1, 3 + num1);
            this._orbiter = new SpriteMap(GetPath("shrekTinySmokeTestFront"), 16, 16, false);
            int num2 = Rando.Int(3) * 4;
            this._orbiter.AddAnimation("idle", 0.1f, 1 != 0, num2);
            this._orbiter.AddAnimation("puff", Rando.Float(0.15f, 0.25f), 0 != 0, num2, 1 + num2, 2 + num2, 3 + num2);
            this._sprite2 = new SpriteMap(GetPath("shrekTinySmokeTestBack"), 16, 16, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
        }

        public static shrekSmallSmoke New(float xpos, float ypos, float depth = 0.8f, float scaleMul = 1f)
        {
            shrekSmallSmoke shrekSmallSmoke;
            if (shrekSmallSmoke._objects[shrekSmallSmoke._lastActiveObject] == null)
            {
                shrekSmallSmoke = new shrekSmallSmoke();
                shrekSmallSmoke._objects[shrekSmallSmoke._lastActiveObject] = shrekSmallSmoke;
            }
            else
                shrekSmallSmoke = shrekSmallSmoke._objects[shrekSmallSmoke._lastActiveObject];
            shrekSmallSmoke._lastActiveObject = (shrekSmallSmoke._lastActiveObject + 1) % shrekSmallSmoke.kMaxObjects;
            shrekSmallSmoke.Init(xpos, ypos);
            shrekSmallSmoke.ResetProperties();
            shrekSmallSmoke._sprite.globalIndex = (int)Thing.GetGlobalIndex();
            shrekSmallSmoke.globalIndex = Thing.GetGlobalIndex();
            shrekSmallSmoke.depth = (Depth)depth;
            shrekSmallSmoke.s1 *= scaleMul;
            shrekSmallSmoke.s2 *= scaleMul;
            if (shrekSmallSmoke.shortlife)
                shrekSmallSmoke.lifeTake = 0.14f;
            return shrekSmallSmoke;
        }

        public static shrekSmallSmoke New(float xpos, float ypos)
        {
            shrekSmallSmoke shrekSmallSmoke;
            if (shrekSmallSmoke._objects[shrekSmallSmoke._lastActiveObject] == null)
            {
                shrekSmallSmoke = new shrekSmallSmoke();
                shrekSmallSmoke._objects[shrekSmallSmoke._lastActiveObject] = shrekSmallSmoke;
            }
            else
                shrekSmallSmoke = shrekSmallSmoke._objects[shrekSmallSmoke._lastActiveObject];
            shrekSmallSmoke._lastActiveObject = (shrekSmallSmoke._lastActiveObject + 1) % shrekSmallSmoke.kMaxObjects;
            shrekSmallSmoke.Init(xpos, ypos);
            shrekSmallSmoke.ResetProperties();
            shrekSmallSmoke._sprite.globalIndex = (int)Thing.GetGlobalIndex();
            shrekSmallSmoke.globalIndex = Thing.GetGlobalIndex();
            shrekSmallSmoke.depth = (Depth)0.8f;
            return shrekSmallSmoke;
        }

        private void Init(float xpos, float ypos)
        {
            this._orbitInc += 0.2f;
            this._life = 1f;
            this.position.x = xpos;
            this.position.y = ypos;
            this._sprite.SetAnimation("idle");
            this._sprite.angleDegrees = Rando.Float(360f);
            this._orbiter.angleDegrees = Rando.Float(360f);
            this.s1 = Rando.Float(3f, 5f);
            this.s2 = Rando.Float(3f, 5f);
            this.hSpeed = Rando.Float(-0.15f, 0.15f);
            this.vSpeed = Rando.Float(-0.15f, 0.1f);
            this._life += Rando.Float(0.2f);
            float num1 = 0.6f - Rando.Float(0.2f);
            float num2 = 0.7f;
            this._sprite.color = new Color(num2, num2, num2);
            this.depth = (Depth)0.8f;
            this.alpha = 1f;
            this.layer = Layer.Game;
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            this.xscale = 2f;
            this.yscale = this.xscale;
            this._orbitInc += this._rotSpeed;
            this._distPulse += this._distPulseSpeed;
            this.vSpeed -= 0.01f;
            this.hSpeed *= 0.95f;
            this._life -= this.lifeTake;
            if ((double)this._life < 0.0 && this._sprite.currentAnimation != "puff")
                this._sprite.SetAnimation("puff");
            if (this._sprite.currentAnimation == "puff" && this._sprite.finished)
                Level.Remove((Thing)this);
            this.x += (float)(double)this.hSpeed;
            this.y += (float)(double)this.vSpeed;
        }

        public override void Draw()
        {
            float num1 = (float)Math.Sin((double)this._distPulse);
            float num2 = (float)-(Math.Sin((double)this._orbitInc) * (double)num1) * this.s1;
            float num3 = (float)Math.Cos((double)this._orbitInc) * num1 * this.s1;
            this._sprite.imageIndex = this._sprite.imageIndex;
            this._sprite.depth = this.depth;
            this._sprite.scale = new Vec2(this.s1);
            this._sprite.center = this.center;
            Graphics.Draw((Sprite)this._sprite, this.x + num2, this.y + num3);
            this._sprite2.imageIndex = this._sprite.imageIndex;
            this._sprite2.angle = this._sprite.angle;
            this._sprite2.scale = this._sprite.scale;
            this._sprite2.center = this.center;
            float num4 = 0.6f - Rando.Float(0.2f);
            float num5 = 0.4f;
            this._sprite2.color = new Color(num5, num5, num5);
            Graphics.Draw((Sprite)this._sprite2, this.x + num2, this.y + num3);
            this._orbiter.imageIndex = this._sprite.imageIndex;
            this._orbiter.color = this._sprite.color;
            this._orbiter.depth = this.depth;
            this._orbiter.scale = new Vec2(this.s2);
            this._orbiter.center = this.center;
            Graphics.Draw((Sprite)this._orbiter, this.x - num2, this.y - num3);
            this._sprite2.imageIndex = this._orbiter.imageIndex;
            this._sprite2.angle = this._orbiter.angle;
            this._sprite2.scale = this._orbiter.scale;
            this._sprite2.center = this.center;
            this._sprite2.color = new Color(num5, num5, num5);
            Graphics.Draw((Sprite)this._sprite2, this.x - num2, this.y - num3);
        }
    }
}
