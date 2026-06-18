using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Environment")]
    public class bigIceWedge : MaterialThing
    {
        public bigIceWedge(float xpos, float ypos, int dir)
          : base(xpos, ypos)
        {
            this._canFlipVert = true;
            this.graphic = (Sprite)new SpriteMap(GetPath("bigIceWedge"), 34, 34, false);
            this.hugWalls = WallHug.Left | WallHug.Right | WallHug.Floor;
            this.center = new Vec2(16f, 28f);
            this.collisionSize = new Vec2(28f, 8f);
            this.collisionOffset = new Vec2(-14f, -4f);
            this._editorName = "Big Ice Wedge";
            this.editorTooltip = "Like a regular ice ramp, but bigger.";
        }
        public override void EditorUpdate()
        {
            base.EditorUpdate();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this.flipVertical)
            {
                if ((double)with.vSpeed < -1.0 && ((int)this.offDir > 0 && (double)with.hSpeed < 1.0 || (int)this.offDir < 0 && (double)with.hSpeed >= -1.0))
                    with.hSpeed = (float)(-(double)with.vSpeed * 1.5) * (float)this.offDir;
                else if (((int)this.offDir < 0 && (double)with.right > (double)this.left + 4.0 || (int)this.offDir > 0 && (double)with.left < (double)this.right - 4.0) && ((int)this.offDir > 0 && (double)with.hSpeed < -1.0 || (int)this.offDir < 0 && (double)with.hSpeed > 1.0) && (double)with.vSpeed < 0.5)
                    with.vSpeed = Math.Abs(with.hSpeed * 1.6f);
            }
            else if ((double)with.vSpeed > 1.0 && ((int)this.offDir > 0 && (double)with.hSpeed < 1.0 || (int)this.offDir < 0 && (double)with.hSpeed >= -1.0))
                with.hSpeed = with.vSpeed * 1.5f * (float)this.offDir;
            else if (((int)this.offDir < 0 && (double)with.right > (double)this.left + 4.0 || (int)this.offDir > 0 && (double)with.left < (double)this.right - 4.0) && ((int)this.offDir > 0 && (double)with.hSpeed < -1.0 || (int)this.offDir < 0 && (double)with.hSpeed > 1.0) && (double)with.vSpeed > -0.5)
                with.vSpeed = -Math.Abs(with.hSpeed * 1.6f);
            base.OnSoftImpact(with, from);
        }

        public override void Draw()
        {
            this.hugWalls = WallHug.None;
            if (this.flipVertical)
                this.hugWalls |= WallHug.Ceiling;
            else
                this.hugWalls |= WallHug.Floor;
            if (this.flipHorizontal)
                this.hugWalls |= WallHug.Right;
            else
                this.hugWalls |= WallHug.Left;
            this.angleDegrees = 0.0f;
            if (this.flipVertical)
            {
                if (this.flipHorizontal)
                {
                    this.angleDegrees = 180f;
                    this.center = new Vec2(16f, 28f);
                    this.collisionSize = new Vec2(28f, 8f);
                    this.collisionOffset = new Vec2(-14f, -4f);
                }
                else
                {
                    this.angleDegrees = 90f;
                    this.center = new Vec2(6f, 18f);
                    this.collisionSize = new Vec2(28f, 8f);
                    this.collisionOffset = new Vec2(-14f, -4f);
                }
            }
            else if (this.flipHorizontal)
            {
                this.angleDegrees = 270f;
                this.center = new Vec2(6f, 18f);
                this.collisionSize = new Vec2(28f, 8f);
                this.collisionOffset = new Vec2(-14f, -4f);
            }
            else
            {
                this.angleDegrees = 0.0f;
                this.center = new Vec2(16f, 28f);
                this.collisionSize = new Vec2(28f, 8f);
                this.collisionOffset = new Vec2(-14f, -4f);
            }
            base.Draw();
        }
    }
}