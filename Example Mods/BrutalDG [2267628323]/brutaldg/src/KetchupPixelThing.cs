using System;
using DuckGame;

namespace DuckGame.BrutalDG
{
    internal class KetchupPixelThing : Thing
    {
        public KetchupPixelThing(Thing t, Vec2 offset, Color c, BloodThings blood) : base()
        {
            this._thing = t;
            this._blood = blood;
            this._offset = offset;
            this.graphic = new Sprite(GetPath("verylargeimage"), 0f, 0f);
            this._layer = t.layer;
            this._collisionSize = new Vec2(1f, 1f);
            this._collisionOffset = new Vec2(0f, 0f);
            this.center = new Vec2(0f, 0f);
            this.graphic.center = new Vec2(0f, 0f);
            this.graphic.color = c;
        }

        public override void Update()
        {
            base.Update();
            if (this._blood == null || this._blood.removeFromLevel)
            {
                Level.Remove(this);
                return;
            }
            if (!this._blood._ketchup.ContainsKey(this))
            {
                Level.Remove(this);
            }
        }

        public void Wash(bool clear = true, bool fast = false)
        {
            this.alpha -= 0.01f;
            if (this.alpha <= 0.02f || fast)
            {
                if (clear)
                {
                    this._blood.remove = true;
                }
                else
                {
                    this._blood._ketchup.Remove(this);
                }
                this._blood._blood.Remove(this._offset);
                Level.Remove(this);
            }
        }

        public override void Draw()
        {
            if (this._thing == null || this._thing != null && this._thing.graphic == null || this._blood == null)
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
            this.position = this._thing.Offset(this._offset - new Vec2(this._thing.graphic.centerx + x, this._thing.graphic.centery + y));
            base.Draw();
        }

        private Thing _thing;

        private Vec2 _offset;

        private BloodThings _blood;
    }
}
