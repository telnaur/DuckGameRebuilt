using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{

    [EditorGroup("Zyrafa|Equipment")]
    public class boomChestPlate : ChestPlate
    {
        private SpriteMap _sprite;
        private SpriteMap _spriteOver;
        private Sprite _pickupSprite;
        private Sprite _mineFlash;
        public bool _pin = false;

        public boomChestPlate(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap(GetPath("boomChestPlateAnim"), 32, 32, false);
            this._spriteOver = new SpriteMap(GetPath("boomChestPlateAnimOver"), 32, 32, false);
            this._pickupSprite = new Sprite(GetPath("boomChestPlatePickup"), 0.0f, 0.0f);
            this._pickupSprite.CenterOrigin();
            this.graphic = this._pickupSprite;
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(11f, 8f);
            this._equippedCollisionOffset = new Vec2(-7f, -8f);
            this._equippedCollisionSize = new Vec2(12f, 18f);
            this._hasEquippedCollision = true;
            this.center = new Vec2(8f, 8f);
            this.physicsMaterial = PhysicsMaterial.Plastic;
            this._equippedDepth = 2;
            this._wearOffset = new Vec2(1f, 1f);
            this._isArmor = true;
            this._equippedThickness = 3f;
            this._mineFlash = new Sprite("mineFlash", 0.0f, 0.0f);
            this._mineFlash.CenterOrigin();
            this._mineFlash.alpha = 0.0f;
            this._editorName = "Boom Chestplate";
            this.editorTooltip = "Put this on and other ducks will think twice before shooting at you within explosion distance.";
        }

        public override void Update()
        {
            base.Update();
            if (this._equippedDuck != null && this.duck == null)
                return;
            if (this._equippedDuck != null && !this.destroyed)
            {

                this._mineFlash.alpha = Lerp.Float(this._mineFlash.alpha, 0.4f, 0.08f);

                this.center = new Vec2(16f, 16f);
                this.solid = false;
                this._sprite.flipH = this.duck._sprite.flipH;
                this._spriteOver.flipH = this.duck._sprite.flipH;
                this.graphic = (Sprite)this._sprite;

            }
            else
            {
                this.center = new Vec2((float)(this._pickupSprite.w / 2), (float)(this._pickupSprite.h / 2));
                this.solid = true;
                this._sprite.frame = 0;
                this._sprite.flipH = false;
                this.graphic = this._pickupSprite;
                this._mineFlash.alpha = Lerp.Float(this._mineFlash.alpha, 0.0f, 0.08f);
            }
            if (this.destroyed)
            {
                this._mineFlash.alpha = Lerp.Float(this._mineFlash.alpha, 0.0f, 0.08f);
                this.alpha -= 0.05f;
                if (this._pin == false)
                {
                    for (int index = 0; index < 20; ++index)
                    {
                        float ang = (float)((double)index * 18.0 - 5.0) + Rando.Float(10f);
                        ATShrapnel atShrapnel = new ATShrapnel();
                        atShrapnel.range = 80f + Rando.Float(18f);
                        atShrapnel.penetration = 2f;
                        Bullet bullet = new Bullet(x, y, (AmmoType)atShrapnel, ang, (Thing)null, false, -1f, false, true);
                        bullet.firedFrom = (Thing)this;
                        Level.Add((Thing)bullet);
                        Level.Add((Thing)new ExplosionPart(x, y, true));
                    }
                    SFX.Play("explode", 1f, 0.0f, 0.0f, false);
                    Graphics.FlashScreen();
                    this._pin = true;
                }
            }
            if ((double)this.alpha < 0.0)
            {
                Level.Remove((Thing)this);
            }
        }
        public override void Removed()
        {

            base.Removed();
        }

        public override void Draw()
        {

            if ((double)this._mineFlash.alpha > 0.00999999977648258)
                Graphics.Draw(this._mineFlash, this.x, this.y - 3f);
            base.Draw();
            if (this._equippedDuck != null && this.duck == null || this._equippedDuck == null)
                return;
            this._spriteOver.flipH = this.graphic.flipH;
            this._spriteOver.angle = this.angle;
            this._spriteOver.alpha = this.alpha;
            this._spriteOver.scale = this.scale;
            this._spriteOver.depth = this.owner.depth + (this.duck.holdObject != null ? 5 : 14);
            this._spriteOver.center = this.center;
            Graphics.Draw((Sprite)this._spriteOver, this.x, this.y);

        }
        public override void Equip(Duck d)
        {
            base.Equip(d);
        }
        public override void UnEquip()
        {
            base.UnEquip();
        }
    }
}
