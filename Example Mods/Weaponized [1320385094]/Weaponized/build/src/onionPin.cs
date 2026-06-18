using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class onionPin : EjectedShell
    {
        private SpriteMap _sprite;
        public onionPin(float xpos, float ypos)
          : base(xpos, ypos, "onionPin", "plasticBounce")

        {
            this._sprite = new SpriteMap(GetPath("onionPin"), 8, 8, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(4f, 4f);
        }
    }
}
