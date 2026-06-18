using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Curses")]
    public class curseBody : Equipment
    {
        protected SpriteMap _sprite;
        protected SpriteMap _pickupSprite;
        private bool sound = false;
        private Sprite _greenFlash;
        private float timer = 0f;

        public curseBody(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new SpriteMap(GetPath("cursedGrenade"), 9, 11, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(5f, 6f);
            this.collisionOffset = new Vec2(-5f, -6f);
            this.collisionSize = new Vec2(9f, 11f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this._sprite = new SpriteMap(GetPath("curseBody"), 32, 32, false);
            this.graphic = this._pickupSprite;
            this._equippedDepth = -1;
            this._greenFlash = new Sprite(GetPath("miniConsoleFlash"), 0.0f, 0.0f);
            this._greenFlash.CenterOrigin();
            this._greenFlash.alpha = 0f;
            this._pickupSprite.AddAnimation("idle", 1f, true, new int[1]);
            this._pickupSprite.AddAnimation("fire", 0.8f, false, 0, 1, 2, 3, 4, 0);
            this._pickupSprite.SetAnimation("idle");
            this._editorName = "Cursed Grenade";
            this.editorTooltip = "Pulling the pin infuses the holder with the power of turning everything they touch into grenades. Everything.";
        }

        public override void Update()
        {
            if (this._equippedDuck != null && !this.destroyed)
            {
                if (sound == false)
                {
                    SFX.Play("convert", 0.8f, 0.0f, 0.0f, false);
                    sound = true;
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
                PhysicsObject holdObject = (PhysicsObject)this.equippedDuck.holdObject;
                if (holdObject != null && !(duck.holdObject is Grenade) && !(duck.holdObject is RagdollPart))
                {
                    duck.ThrowItem(false);
                    Level.Remove((PhysicsObject)holdObject);
                    for (int index = 0; index < 4; ++index)
                        Level.Add((Thing)SmallSmoke.New(this.x + Rando.Float(-2f, 2f), this.y + Rando.Float(-2f, 2f)));
                    SFX.Play("ignite", 0.8f, 0.0f, 0.0f, false);
                    Grenade grenade = new Grenade(this.x, this.y);
                    Level.Add((PhysicsObject)grenade);
                    if (duck == null)
                        return;
                    duck.GiveHoldable(grenade);
                }
                this._greenFlash.alpha = Lerp.Float(this._greenFlash.alpha, 0.6f, 0.06f);
            }
            else
            {
               // if (this._pickupSprite.currentAnimation == "fire" && this._pickupSprite.frame == 6)
                this.graphic = this._pickupSprite;
                this.center = new Vec2(5f, 6f);
                this.collisionOffset = new Vec2(-5f, -6f);
                this.collisionSize = new Vec2(9f, 11f);
                this.solid = true;
                this._sprite.frame = 0;
                this._sprite.flipH = false;
                this.timer += 0.2f;
                if (timer >= 50f)
                {
                    this._greenFlash.alpha = Lerp.Float(this._greenFlash.alpha, 0.6f, 0.06f);
                }
                if (this._greenFlash.alpha >= 0.35f)
                {
                    //this._pickupSprite.SetAnimation("fire");
                    this._greenFlash.alpha = Lerp.Float(this._greenFlash.alpha, 0.0f, 0.6f);
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
            if ((double)this._greenFlash.alpha > 0.00999999977648258)
                Graphics.Draw(this._greenFlash, this.x, this.y + 1f);
            base.Draw();
        }
    }
}