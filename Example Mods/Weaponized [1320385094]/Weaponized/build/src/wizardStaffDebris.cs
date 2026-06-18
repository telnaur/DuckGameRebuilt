using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class wizardStaffDebris : PhysicsParticle
    {
        private static int kMaxObjects = 64;
        private static wizardStaffDebris[] _objects = new wizardStaffDebris[wizardStaffDebris.kMaxObjects];
        private static int _lastActiveObject = 0;
        private SpriteMap _sprite;
        public wizardStaffDebris()
          : base(0.0f, 0.0f)
        {
            this._sprite = new SpriteMap(GetPath("wizardStaffDebris"), 8, 8, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(4f, 4f);
        }

        public static wizardStaffDebris New(float xpos, float ypos)
        {
            wizardStaffDebris wizardStaffDebris;
            if (wizardStaffDebris._objects[wizardStaffDebris._lastActiveObject] == null)
            {
                wizardStaffDebris = new wizardStaffDebris();
                wizardStaffDebris._objects[wizardStaffDebris._lastActiveObject] = wizardStaffDebris;
            }
            else
                wizardStaffDebris = wizardStaffDebris._objects[wizardStaffDebris._lastActiveObject];
            wizardStaffDebris._lastActiveObject = (wizardStaffDebris._lastActiveObject + 1) % wizardStaffDebris.kMaxObjects;
            wizardStaffDebris.ResetProperties();
            wizardStaffDebris.Init(xpos, ypos);
            wizardStaffDebris._sprite.globalIndex = (int)Thing.GetGlobalIndex();
            wizardStaffDebris.globalIndex = Thing.GetGlobalIndex();
            return wizardStaffDebris;
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
