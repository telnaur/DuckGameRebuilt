using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Environment")]
    public class railLights : Thing
    {
        private Sprite _mineFlash;
        private Sprite _mineFlash2;
        private SpriteMap _sprite;
        public railLights(float xpos, float ypos)
          : base(xpos, ypos, (Sprite)null)
        {
            this._sprite = new SpriteMap((GetPath("railLights")), 30, 61, false);
            this.center = new Vec2(15f, 51f);
            this.graphic = (Sprite)this._sprite;
            this._collisionSize = new Vec2(20f, 20f);
            this._collisionOffset = new Vec2(-10f, -10f);
            this.depth = (Depth)0.9f;
            this.hugWalls = WallHug.Floor;
            this._mineFlash = new Sprite("mineFlash", 0.0f, 0.0f);
            this._mineFlash.CenterOrigin();
            this._mineFlash.alpha = 0.4f;
            //this._mineFlash.depth = (Depth)0.9f;
            this._mineFlash2 = new Sprite("mineFlash", 0.0f, 0.0f);
            this._mineFlash2.CenterOrigin();
            //this._mineFlash2.depth = (Depth)0.9f;
            this._mineFlash2.alpha = 0f;
            this._sprite.AddAnimation("exist", 0.03f, true, 1, 0);
            this._sprite.AddAnimation("none", 0.05f, false, 0);
            this._sprite.SetAnimation("exist");
            this._editorName = "Railway Lights";
            this.editorTooltip = "These don't serve much informative purpose, they blink all the time.";
        }

        public override void Update()
        {
            if (Level.current is Editor)
                return;
            else
            {
                if (this._sprite.frame == 0)
                {
                    this._mineFlash.alpha = Lerp.Float(this._mineFlash.alpha, 0.35f, 0.07f);
                    this._mineFlash2.alpha = Lerp.Float(this._mineFlash2.alpha, 0.0f, 0.07f);
                }
                else
                {
                    this._mineFlash.alpha = Lerp.Float(this._mineFlash.alpha, 0.0f, 0.07f);
                    this._mineFlash2.alpha = Lerp.Float(this._mineFlash2.alpha, 0.35f, 0.07f);
                }
                base.Update();
            }
        }
        public override void EditorRender()
        {
            this._sprite.SetAnimation("none");
            base.EditorRender();
        }
        public override void Draw()
        {
            if (Level.current is Editor)
            {
                base.Draw();
                return;
            }
            else
            {
                if ((double)this._mineFlash.alpha > 0.00999999977648258)
                    Graphics.Draw(this._mineFlash, this.x - 6f, this.y - 28f);
                if ((double)this._mineFlash2.alpha > 0.00999999977648258)
                    Graphics.Draw(this._mineFlash2, this.x + 4f, this.y - 28f);
                base.Draw();
            }
        }
    }
}