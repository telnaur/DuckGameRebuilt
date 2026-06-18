using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class potionDebris : PhysicsParticle
    {
        private static int kMaxObjects = 64;
        private static potionDebris[] _objects = new potionDebris[potionDebris.kMaxObjects];
        private static int _lastActiveObject = 0;
        private SpriteMap _sprite;
        public potionDebris()
          : base(0.0f, 0.0f)
        {
            this._sprite = new SpriteMap(GetPath("potionDebris"), 8, 8, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(4f, 4f);
        }

        public static potionDebris New(float xpos, float ypos)
        {
            potionDebris potionDebris;
            if (potionDebris._objects[potionDebris._lastActiveObject] == null)
            {
                potionDebris = new potionDebris();
                potionDebris._objects[potionDebris._lastActiveObject] = potionDebris;
            }
            else
                potionDebris = potionDebris._objects[potionDebris._lastActiveObject];
            potionDebris._lastActiveObject = (potionDebris._lastActiveObject + 1) % potionDebris.kMaxObjects;
            potionDebris.ResetProperties();
            potionDebris.Init(xpos, ypos);
            potionDebris._sprite.globalIndex = (int)Thing.GetGlobalIndex();
            potionDebris.globalIndex = Thing.GetGlobalIndex();
            return potionDebris;
        }

        private void Init(float xpos, float ypos)
        {
            this.position.x = xpos;
            this.position.y = ypos;
            this.hSpeed = -4f - Rando.Float(3f);
            this.vSpeed = (float)-((double)Rando.Float(1.5f) + 1.0);
            this._sprite.frame = Rando.Int(4);
            this._bounceEfficiency = 0.3f;
        }



        public override void Update()
        {
            base.Update();
        }
    }
}
