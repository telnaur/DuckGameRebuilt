using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public abstract class ejectedSeekerMagazine : PhysicsParticle
    {
        private SpriteMap _sprite;

        protected ejectedSeekerMagazine(float xpos, float ypos, string shellSprite, string bounceSound = "metalBounce")
          : base(xpos, ypos)
        {
            this.hSpeed = 0f - Rando.Float(1f);
            this.vSpeed = 1f - Rando.Float(2f);
            this._sprite = new SpriteMap(GetPath("seekerMagazine"), 10, 10, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(5f, 5f);
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