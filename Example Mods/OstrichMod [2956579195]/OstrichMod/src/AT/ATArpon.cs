using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    public class ATArpon : PhysicsObject, IPlatform
    {
        public StateBinding _stickTimeBinding = new StateBinding("_stickTime", -1, false);
        public StateBinding _stuckBinding = new StateBinding("_stuck", -1, false);
        public float _stickTime = 1f;
        private SpriteMap _sprite;
        public bool _stuck;
        private Duck _owner;
        public bool burning;

        public ATArpon(float xpos, float ypos, Duck owner, float fireAngle)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap(this.GetPath("Rail"), 31, 7, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(15f, 3f);
            this.collisionOffset = new Vec2(-15f, -2f);
            this.collisionSize = new Vec2(31f, 1f);
            this.thickness = 1f;
            this.weight = 15f;
            this._owner = owner;
            this.breakForce = 1000f;
            this.angle = fireAngle;
            this.gravMultiplier = 0f;
            this.friction = 0f;
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._stuck || with is Gun || (double)with.weight < 5.0 && !(with is ATArpon) || (with is FeatherVolume || with is Teleporter || (this.destroyed || this._stuck)))
                return;
            if (with is Duck)
            {
                ((Duck)with).Kill((DestroyType)new DTImpale((Thing)with));
                Level.Remove((this));
            }
            if (with is Door)
            {
                Level.Remove((this));
            }
            this._stuck = true;
            this.vSpeed = 0.0f;
            this.gravMultiplier = 0.0f;
            this.grounded = true;
            this._sprite.frame = 1;
            this._stickTime = 1f;
        }

        public override void Update()
        {
            base.Update();
            if (this.hSpeed == 0f)
            {
                this._stuck = true;
            }
            if (!this.destroyed && !this._stuck)
            {
                this._sprite.frame = 0;
                this.angleDegrees = -Maths.PointDirection(Vec2.Zero, new Vec2(this.hSpeed, this.vSpeed));
            }
            if (this._stuck)
            {
                this.vSpeed = 0.0f;
                this.hSpeed = 0.0f;
                this.grounded = true;
                this._sprite.frame = 1;
                this._stickTime -= 0.01f;
                this.gravMultiplier = 0.0f;
            }
        }
    }
}
