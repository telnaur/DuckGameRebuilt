using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Pistols")]
    public class nailPistol : Gun
    {
        private int _ammoMax = 5;
        private SpriteMap _sprite;
        public nailPistol(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = this._ammoMax;
            this._ammoType = (AmmoType)new ATnail();
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("nailPistol"), 16, 10);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 5f);
            this.collisionOffset = new Vec2(-8f, -5f);
            this.collisionSize = new Vec2(16f, 10f);
            this._barrelOffsetTL = new Vec2(16f, 1f);
            this._fireSound = "littleGun";
            this._kickForce = 1f;
            this._holdOffset = new Vec2(0f, 2f);
            this._editorName = "Nail Pistol";
            this.editorTooltip = "Take gravity into account and the nails will reach your target.";
        }
        public override void Update()
        {
            if (this.ammo == 0)
                this._sprite.frame = 5;
            else
                this._sprite.frame = this._ammoMax - this.ammo;
            
            base.Update();
        }
    }
}
