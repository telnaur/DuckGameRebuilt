using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{ 
    public class lilFeather : Thing
    {
        private static int kMaxObjects = 64;
        private static lilFeather[] _objects = new lilFeather[lilFeather.kMaxObjects];
        private static int _lastActiveObject = 0;
        private SpriteMap _sprite;
        private bool _rested;

        private lilFeather()
          : base(0.0f, 0.0f, (Sprite)null)
        {
            this._sprite = new SpriteMap("Feather", 12, 4, false);
            this._sprite.speed = 0.3f;
            this._sprite.AddAnimation("Feather", 1f, 1 != 0, 0, 1, 2, 3);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(6f, 1f);
        }

        public static lilFeather New(float xpos, float ypos)
        {
            lilFeather LilFeather;
            if (lilFeather._objects[lilFeather._lastActiveObject] == null)
            {
                LilFeather = new lilFeather();
                lilFeather._objects[lilFeather._lastActiveObject] = LilFeather;
            }
            else
                LilFeather = lilFeather._objects[lilFeather._lastActiveObject];
            Level.Remove((Thing)LilFeather);
            lilFeather._lastActiveObject = (lilFeather._lastActiveObject + 1) % lilFeather.kMaxObjects;
            LilFeather.Init(xpos, ypos);
            LilFeather.ResetProperties();
            LilFeather._sprite.globalIndex = (int)Thing.GetGlobalIndex();
            LilFeather.globalIndex = Thing.GetGlobalIndex();
            return LilFeather;
        }

        private void Init(float xpos, float ypos)
        {
            this.position.x = xpos;
            this.position.y = ypos;
            this.alpha = 1f;
            this.hSpeed = Rando.Float(6f) - 3f;
            this.vSpeed = (float)((double)Rando.Float(2f) - 1.0 - 1.0);
            this._sprite.SetAnimation("Feather");
            this._sprite.frame = Rando.Int(3);
            if (Rando.Double() > 0.5)
                this._sprite.flipH = true;
            else
                this._sprite.flipH = false;
            this.graphic = (Sprite)this._sprite;
            this._rested = false;
        }

        public override void Update()
        {
            if (this._rested)
                return;
            if ((double)this.hSpeed > 0.0)
                this.hSpeed -= 0.1f;
            if ((double)this.hSpeed < 0.0)
                this.hSpeed += 0.1f;
            if ((double)this.hSpeed < 0.1 && (double)this.hSpeed > -0.100000001490116)
                this.hSpeed = 0.0f;
            if ((double)this.vSpeed < 1.0)
                this.vSpeed += 0.06f;
            if ((double)this.vSpeed < 0.0)
            {
                this._sprite.speed = 0.0f;
                if ((Thing)Level.CheckPoint<Block>(this.x, this.y - 7f, (Thing)null, (Layer)null) != null)
                    this.vSpeed = 0.0f;
            }
            else
            {
                Thing thing = Level.CheckPoint<IPlatform>(this.x, this.y + 3f, (Thing)null, (Layer)null) as Thing;
                if (thing != null)
                {
                    this.vSpeed = 0.0f;
                    this._sprite.speed = 0.0f;
                    if (thing is Block)
                        this._rested = true;
                }
                else
                    this._sprite.speed = 0.3f;
            }
            this.x += (float)(double)this.hSpeed;
            this.y += (float)(double)this.vSpeed;
        }
    }
}
