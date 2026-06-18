using System;
using DuckGame;

namespace DuckGame.BrutalDG
{
    internal class KetchupPixelBlock : Thing
    {
        public KetchupPixelBlock(Thing t, Vec2 offset, Color c, BloodBlocks blood) : base()
        {
            this._thing = t;
            this._blood = blood;
            this._offset = offset;
            this._layer = t.layer;
            this.graphic = new Sprite(GetPath("verylargeimage"), 0f, 0f);
            this._collisionSize = new Vec2(1f, 1f);
            this._collisionOffset = new Vec2(0f, 0f);
            this.center = new Vec2(0f, 0f);
            this.graphic.center = new Vec2(0f, 0f);
            this.graphic.color = c;
        }

        public override void Draw()
        {
            if (this._thing == null || this._thing != null && this._thing.graphic == null || this._blood == null || this._blood.removeFromLevel)
            {
                Level.Remove(this);
                return;
            }
            this._depth = this._thing.depth.Add(1);
            this.angleDegrees = this._thing.graphic.angleDegrees;
            int x = 0;
            int y = 0;
            if (this._thing.graphic.flipH)
            {
                x = -1;
            }
            if (this._thing.graphic.flipV)
            {
                y = -1;
            }
            this.position = this._thing.position - this._thing.graphic.center + this._offset;
            //this.position = this._thing.Offset(this._offset - new Vec2(this._thing.graphic.centerx + x, this._thing.graphic.centery + y));
            base.Draw();
        }

        public void Wash(bool fast = false)
        {
            this.alpha -= 0.01f;
            if (this.alpha <= 0.02f || fast)
            {
                this._blood._ketchup.Remove(this);
                Level.Remove(this);
            }
        }

        private Thing _thing;

        private Vec2 _offset;

        private BloodBlocks _blood;
    }
}
