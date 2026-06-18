using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class blueScreen : Thing
    {
        private bool _created;
        private float _wait;
        private SpriteMap _sprite;

        public blueScreen(float xpos, float ypos, bool doWait = true)
          : base(xpos, ypos, (Sprite)null)
        {
            this.xscale = 1.2f;
            this.yscale = this.xscale;
            this._sprite = new SpriteMap(GetPath("blueScreen"), 23, 30, false);
            this._sprite.AddAnimation("blue", 1f, 0 != 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8);
            this._sprite.SetAnimation("blue");
            this.graphic = (Sprite)this._sprite;
            this._sprite.speed = 0.4f + Rando.Float(0.2f);
            this.xscale = 1f;
            this.yscale = this.xscale;
            this.center = new Vec2(12f, 15f);
            this.depth = (Depth)1f;
            if (doWait)
                return;
            this._wait = 0.0f;
        }
        public override void Initialize()
        {
        }

        public override void Update()
        {
            if (!this._created)
                this._created = true;
            if (!this._sprite.finished)
                return;
            Level.Remove((Thing)this);
        }
        public override void Draw()
        {
            if ((double)this._wait > 0.0)
                this._wait -= 0.2f;
            else
                base.Draw();
        }
    }
}