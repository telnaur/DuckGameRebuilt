using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public abstract class slomoMagnumEjectedShell : PhysicsParticle
    {
        private SpriteMap _sprite;

        protected slomoMagnumEjectedShell(float xpos, float ypos, string shellSprite, string bounceSound = "metalBounce")
          : base(xpos, ypos)
        {
            this.hSpeed = -4f - Rando.Float(3f);
            this.vSpeed = (float)-((double)Rando.Float(1.5f) + 1.0);
            this._sprite = new SpriteMap(GetPath("slomoMagnumShell"), 16, 16, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
            this._bounceSound = bounceSound;
            this.depth = (Depth)(0.3f + Rando.Float(0.0f, 0.1f));
        }

        public override void Update()
        {
            base.Update();
            this._angle = Maths.DegToRad(-this._spinAngle);
        }
    }
}