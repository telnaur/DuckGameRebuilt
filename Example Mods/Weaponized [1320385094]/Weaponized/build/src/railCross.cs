using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Environment")]
    public class railCross : Thing
    {
        public EditorProperty<int> style;
        public railCross(float xpos, float ypos)
          : base(xpos, ypos, (Sprite)null)
        {
            this.style = new EditorProperty<int>(0, (Thing)this, 0f, 1f, 1f, (string)null, false, false);
            this.graphic = (Sprite)new SpriteMap((GetPath("railCross")), 30, 61, false);
            this.center = new Vec2(15f, 50f);
            this._collisionSize = new Vec2(20f, 21f);
            this._collisionOffset = new Vec2(-10f, -10f);
            this.depth = (Depth)0.9f;
            this.hugWalls = WallHug.Floor;
            this._editorName = "Railway Cross";
            this.editorTooltip = "Watch out for the train, it might come any minute now.";
        }


        public override void EditorPropertyChanged(object property)
        {
            (this.graphic as SpriteMap).frame = this.style.value;
        }
        public override void Draw()
        {
            this.graphic.flipH = (int)this.offDir <= 0;
            base.Draw();
        }
    }
}
