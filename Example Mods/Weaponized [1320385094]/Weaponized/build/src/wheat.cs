using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Environment")]
    public class wheat : Thing
    {

        public wheat(float xpos, float ypos)
          : base(xpos, ypos, (Sprite)null)
        {
            this.graphic = new Sprite(GetPath("wheat"));
            this.center = new Vec2(24f, 24f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.85f;
            this.hugWalls = WallHug.Left | WallHug.Right | WallHug.Floor;
            this._editorName = "Haystack";
            this.editorTooltip = "Searching for a needle, you might find a duck.";
        }

        public override void Draw()
        {
            this.graphic.flipH = (int)this.offDir <= 0;
            base.Draw();
        }
    }
}
