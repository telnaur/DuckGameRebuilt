using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Curses")]
    public class curseBanana : Equipment
    {
        protected SpriteMap _sprite;
        protected SpriteMap _pickupSprite;
        private bool sound = false;
        private Sprite _yellowFlash;
        private float timer = 0f;

        public curseBanana(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new SpriteMap(GetPath("curseBanana"), 15, 14, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 7f);
            this.collisionOffset = new Vec2(-8f, -7f);
            this.collisionSize = new Vec2(15f, 14f);
            this._holdOffset = new Vec2(0.0f, 2f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this._sprite = new SpriteMap(GetPath("curseBodyBanana"), 32, 32, false);
            this.graphic = this._pickupSprite;
            this._equippedDepth = -1;
            this._yellowFlash = new Sprite(GetPath("miniBananaFlash"), 0.0f, 0.0f);
            this._yellowFlash.CenterOrigin();
            this._yellowFlash.alpha = 0f;
            this._pickupSprite.AddAnimation("idle", 1f, true, new int[1]);
            this._pickupSprite.AddAnimation("fire", 0.8f, false, 0, 1, 2, 3, 4, 5, 6, 0);
            this._pickupSprite.SetAnimation("idle");
            this._editorName = "Cursed Banana";
            this.editorTooltip = "Once consumed, gives the unlucky duck uncontrollable spasms until someone ends its miserable life.";
        }

        public override void Update()
        {
            if (this._equippedDuck != null && !this.destroyed)
            {
                if (sound == false)
                {
                    SFX.Play("convert", 0.8f, 0.0f, 0.0f, false);
                    sound = true;
                    timer = 0f;
                    SFX.Play("slip", 0.6f, 0.0f, 0.0f, false);
                    duck.crippleTimer = 1.3f;
                    if (Rando.Int(0, 1) == 1)
                        duck.hSpeed = Rando.Float(2f, 3f);
                    else
                        duck.hSpeed = Rando.Float(2f, 3f) * -1f;
                    duck.GoRagdoll();
                }
                this.center = new Vec2(16f, 16f);
                this.graphic = (Sprite)this._sprite;
                this.collisionOffset = new Vec2(0.0f, -9999f);
                this.collisionSize = new Vec2(0.0f, 0.0f);
                this.solid = false;
                this._sprite.frame = this._equippedDuck._sprite.imageIndex;
                if (this._equippedDuck.ragdoll != null)
                    this._sprite.frame = 18;
                this._sprite.flipH = this._equippedDuck._sprite.flipH;
                timer -= Rando.Float(0.08f, 0.1f);
                if (timer <= -22f) {
                    duck.crippleTimer = 1.3f;
                    if (Rando.Int(0, 1) == 1)
                        duck.hSpeed = Rando.Float(2f, 3f);
                    else
                        duck.hSpeed = Rando.Float(2f, 3f) * -1f;
                    duck.GoRagdoll();
                    timer = 0f;
                    SFX.Play("slip", 0.6f, 0.0f, 0.0f, false);
                }
                this._yellowFlash.alpha = Lerp.Float(this._yellowFlash.alpha, 0.6f, 0.06f);
            }
            else
            {
                // if (this._pickupSprite.currentAnimation == "fire" && this._pickupSprite.frame == 6)
                this.graphic = this._pickupSprite;
                this.center = new Vec2(8f, 7f);
                this.collisionOffset = new Vec2(-8f, -7f);
                this.collisionSize = new Vec2(15f, 14f);
                this.solid = true;
                this._sprite.frame = 0;
                this._sprite.flipH = false;
                this.timer += 0.2f;
                if (timer >= 50f)
                {
                    this._yellowFlash.alpha = Lerp.Float(this._yellowFlash.alpha, 0.6f, 0.06f);
                }
                if (this._yellowFlash.alpha >= 0.6f)
                {
                    //this._pickupSprite.SetAnimation("fire");
                    this._yellowFlash.alpha = Lerp.Float(this._yellowFlash.alpha, 0.0f, 0.6f);
                    this.timer = -2.5f;
                }
                if (this.timer == -2.5f)
                {
                    this._pickupSprite.SetAnimation("idle");
                    this._pickupSprite.SetAnimation("fire");
                }
            }
            if (this.destroyed)
                this.alpha -= 0.05f;
            if ((double)this.alpha < 0.0)
                Level.Remove((Thing)this);
            base.Update();
        }
        public override void Draw()
        {
            if ((double)this._yellowFlash.alpha > 0.00999999977648258)
                Graphics.Draw(this._yellowFlash, this.x, this.y + 1f);
            base.Draw();
        }
    }
}