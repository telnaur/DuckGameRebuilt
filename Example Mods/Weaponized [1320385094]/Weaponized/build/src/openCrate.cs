using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;
namespace MyMod.src
{
    [EditorGroup("Zyrafa|Stuff")]
    public class openCrate : Thing
    {
        public EditorProperty<int> style;
        public openCrate(float xpos, float ypos)
          : base(xpos, ypos, (Sprite)null)
        {
            this.style = new EditorProperty<int>(-1, (Thing)this, 0f, 3f, 1f, (string)null, false, false);
            this.graphic = (Sprite)new SpriteMap((GetPath("openCrate")), 27, 25, false);
            if ((int)this.style == -1)
                (this.graphic as SpriteMap).frame = Rando.Int(4);
            this.center = new Vec2(14f, 13f);
            this._collisionSize = new Vec2(27f, 25f);
            this._collisionOffset = new Vec2(-14f, -13f);
            this.depth = (Depth)0.9f;
            this.hugWalls = WallHug.Floor;
            this._editorName = "Open Crate";
            this.editorTooltip = "Let's see how many ducks can fit into this huge crate.";
        }

        public override void Initialize()
        {
            Vec2 vec2_wall = this.Offset(new Vec2(0f, 0f));
            openCrateWallBottom bottom = new openCrateWallBottom(vec2_wall.x, vec2_wall.y + 11f);
            Level.Add((Thing)bottom);
            openCrateWallL left = new openCrateWallL(vec2_wall.x - 11f, vec2_wall.y);
            Level.Add((Thing)left);
            openCrateWallR right = new openCrateWallR(vec2_wall.x + 11f, vec2_wall.y);
            Level.Add((Thing)right);
            base.Initialize();
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
