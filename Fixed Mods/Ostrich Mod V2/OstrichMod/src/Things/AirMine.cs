using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    public class AirMine : PhysicsObject
    {
        private SpriteMap _sprite;

        public AirMine(float xpos, float ypos, Duck owner, float fireAngle)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap(this.GetPath("ATAerMine"), 16, 16, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.thickness = 1f;
            this.angle = fireAngle;
            this.gravMultiplier = 0f;
            this.friction = 0.3f;
        }
        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is IAmADuck || with is Holdable && with is IPlatform)
            {
                GrenadeExplosion exp = new GrenadeExplosion(0, 0);
                exp.position = this.position;
                exp.hSpeed = this.x * 7f;
                exp.vSpeed = this.y * 7f;
                Level.Add((Thing)exp);
                Level.Remove((this));
            }
        }
    }
}
