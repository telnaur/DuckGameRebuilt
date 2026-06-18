using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Environment")]
    public class streetLamp : Thing
    {

        public streetLamp(float xpos, float ypos)
          : base(xpos, ypos, (Sprite)null)
        {
            this.graphic = new Sprite(GetPath("streetLamp"));
            this.center = new Vec2(24f, 72f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.85f;
            this.hugWalls = WallHug.Left | WallHug.Right | WallHug.Floor;
            this._editorName = "Street Lamp";
            this.editorTooltip = "Ducks can hide behind it like in cartoons.";
        }
        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Level.Add((Thing)new PointLight(this.x, this.y - 40f, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue), 100f, (List<LightOccluder>)null, true));
        }
        public override void Draw()
        {
            this.graphic.flipH = (int)this.offDir <= 0;
            base.Draw();
        }
    }
}

