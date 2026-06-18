using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Environment")]
    public class Fence : Thing
    {
        public EditorProperty<int> style;
        public Fence(float xpos, float ypos)
          : base(xpos, ypos, (Sprite)null)
        {
            this.style = new EditorProperty<int>(-1, (Thing)this, 0f, 3f, 1f, (string)null, false, false);
            this.graphic = (Sprite)new SpriteMap((GetPath("fences")), 48, 48, false);
            if ((int)this.style == -1)
                (this.graphic as SpriteMap).frame = Rando.Int(4);
            this.center = new Vec2(24f, 24f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.9f;
            this.hugWalls = WallHug.Floor;
            this._editorName = "Fence";
            this.editorTooltip = "These give off a nice cosy farmland vibe.";
        }


        public override void EditorPropertyChanged(object property)
        {
            if ((int)this.style == -1)
                (this.graphic as SpriteMap).frame = Rando.Int(4);
            else
                (this.graphic as SpriteMap).frame = this.style.value;
        }
        public override void Draw()
        {
            this.graphic.flipH = (int)this.offDir <= 0;
            base.Draw();
        }
    }
}
