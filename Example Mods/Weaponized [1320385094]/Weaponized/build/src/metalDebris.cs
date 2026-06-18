using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class metalDebris : PhysicsParticle
    {
        private static int kMaxObjects = 64;
        private static metalDebris[] _objects = new metalDebris[metalDebris.kMaxObjects];
        private static int _lastActiveObject = 0;
        private SpriteMap _sprite;

        public metalDebris()
          : base(0.0f, 0.0f)
        {
            this._sprite = new SpriteMap(GetPath("metalDebris"), 8, 8, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(4f, 4f);
        }

        public static metalDebris New(float xpos, float ypos)
        {
            metalDebris metalDebris;
            if (metalDebris._objects[metalDebris._lastActiveObject] == null)
            {
                metalDebris = new metalDebris();
                metalDebris._objects[metalDebris._lastActiveObject] = metalDebris;
            }
            else
                metalDebris = metalDebris._objects[metalDebris._lastActiveObject];
            metalDebris._lastActiveObject = (metalDebris._lastActiveObject + 1) % metalDebris.kMaxObjects;
            metalDebris.ResetProperties();
            metalDebris.Init(xpos, ypos);
            metalDebris._sprite.globalIndex = (int)Thing.GetGlobalIndex();
            metalDebris.globalIndex = Thing.GetGlobalIndex();
            return metalDebris;
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
