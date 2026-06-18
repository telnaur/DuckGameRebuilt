using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Magic")]
    public class Elixir : Equipment
    {
        private SpriteMap sprite;
        private Sprite _pickupSprite;

        public Elixir(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._editorName = "Elixir";
            this._pickupSprite = new Sprite(this.GetPath("Elixir"), 0.0f, 0.0f);
            this.sprite = new SpriteMap(this.GetPath("AngelicalWings"), 46, 20, false);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(6f, 6f);
            this._holdOffset = new Vec2(0f, 0f);
            this.collisionOffset = new Vec2(0f, 0f);
            this.collisionSize = new Vec2(6f, 6f);
            this.sprite.CenterOrigin();
            this._equippedThickness = 0.0f;
            this.flammable = 0.0f;
        }

        public override void Update()
        {
            if (this.equippedDuck == null)
            {
                this.graphic = this._pickupSprite;
                this.center = new Vec2(6f, 6f);
            }
            else
             {
                if (this.sprite.frame <= 5)
                    this._equippedDepth = -1;
                else
                    this._equippedDepth = -1;
                this.graphic = (Sprite)this.sprite;
                this.center = new Vec2(24, 15);
                this.wearOffset = new Vec2(0.0f, 0.0f);
            }
            base.Update();
            if (this._equippedDuck == null)
                return;
            if (this._equippedDuck.inputProfile.Down("UP"))
                this.equippedDuck._vSpeed += -0.3f;
        }
    }
}