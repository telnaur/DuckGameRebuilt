using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    public class ATParasite : PhysicsObject
    {
        public StateBinding _countStateBinding = new StateBinding("_count");
        private SpriteMap _sprite;
        private Duck _owner;
        public bool burning;
        public int _count;
        public bool cantMove = true;

        public ATParasite(float xpos, float ypos, Duck owner, float fireAngle)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap(this.GetPath("Dardo"), 9, 3, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(3f, 1f);
            this.collisionOffset = new Vec2(-6f, 1f);
            this.collisionSize = new Vec2(9f, 3f);
            this.thickness = 1f;
            this._owner = owner;
            this.angle = fireAngle;
            this.gravMultiplier = 0f;
            this.friction = 0f;
        }
        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is Duck)
            {
                Level.Add(new StunHandler(with as Duck, 200, showDaze: true));
                Level.Remove((this));
            }
            else if (with is Door || with is Block || with is Window)
            {
                Level.Remove((this));
            }
        }
    }
}
