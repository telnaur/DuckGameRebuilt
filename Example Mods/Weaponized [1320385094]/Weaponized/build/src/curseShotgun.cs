using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Curses")]
    public class curseShotgun : Equipment
    {
        protected SpriteMap _sprite;
        protected SpriteMap _pickupSprite;
        private bool sound = false;
        private Sprite _blueFlash;
        private float timer = 0f;

        public curseShotgun(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new SpriteMap(GetPath("cursedShotgun"), 32, 32, false);
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(16f, 8f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this._sprite = new SpriteMap(GetPath("curseBodyShotgun"), 32, 32, false);
            this.graphic = this._pickupSprite;
            this._equippedDepth = -1;
            this._blueFlash = new Sprite(GetPath("miniShotgunFlash"), 0.0f, 0.0f);
            this._blueFlash.CenterOrigin();
            this._blueFlash.alpha = 0f;
            this._pickupSprite.AddAnimation("idle", 1f, true, new int[1]);
            this._pickupSprite.AddAnimation("fire", 0.8f, false, 0, 1, 2, 3, 4, 5, 0);
            this._pickupSprite.SetAnimation("idle");
            this._editorName = "Cursed Shotgun";
            this.editorTooltip = "Let's hope its user loves sliding on ice. All. The. Time.";
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
                    _equippedDuck.modFric = true;
                }
                _equippedDuck.specialFrictionMod = 0.05f;
                this.center = new Vec2(16f, 16f);
                this.graphic = (Sprite)this._sprite;
                this.collisionOffset = new Vec2(0.0f, -9999f);
                this.collisionSize = new Vec2(0.0f, 0.0f);
                this.solid = false;
                this._sprite.frame = this._equippedDuck._sprite.imageIndex;
                if (this._equippedDuck.ragdoll != null)
                    this._sprite.frame = 18;
                this._sprite.flipH = this._equippedDuck._sprite.flipH;
                this._blueFlash.alpha = Lerp.Float(this._blueFlash.alpha, 0.6f, 0.06f);
            }
            else
            {
                // if (this._pickupSprite.currentAnimation == "fire" && this._pickupSprite.frame == 6)
                this.graphic = this._pickupSprite;
                this.center = new Vec2(16f, 16f);
                this.collisionOffset = new Vec2(-8f, -3f);
                this.collisionSize = new Vec2(16f, 8f);
                this.solid = true;
                this._sprite.frame = 0;
                this._sprite.flipH = false;
                this.timer += 0.2f;
                if (timer >= 50f)
                {
                    this._blueFlash.alpha = Lerp.Float(this._blueFlash.alpha, 0.6f, 0.06f);
                }
                if (this._blueFlash.alpha >= 0.6f)
                {
                    //this._pickupSprite.SetAnimation("fire");
                    this._blueFlash.alpha = Lerp.Float(this._blueFlash.alpha, 0.0f, 0.6f);
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
            if ((double)this._blueFlash.alpha > 0.00999999977648258)
                Graphics.Draw(this._blueFlash, this.x, this.y + 1f);
            base.Draw();
        }
    }
}