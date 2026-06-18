using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Shotguns")]
    public class nailShotgun : Shotgun
    {
        public nailShotgun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = (AmmoType)new ATnail();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("nailShotgun"), 0.0f, 0.0f);
            this.center = new Vec2(11f, 5f);
            this.collisionOffset = new Vec2(-11f, -5f);
            this.collisionSize = new Vec2(21f, 10f);
            this._barrelOffsetTL = new Vec2(21f, 2f);
            this._loaderSprite = new SpriteMap(GetPath("nailShotgunLoader"), 8, 8, false);
            this._loaderSprite.center = new Vec2(4f, 8f);
            this._fireSound = "shotgunFire2";
            this._kickForce = 4f;
            this.ammoType.accuracy = 0.5f;
            this._numBulletsPerFire = 4;
            this._holdOffset = new Vec2(1f, 1f);
            this._editorName = "Nail Shotgun";
            this.editorTooltip = "The increased amount of nails makes it much easier to hit enemies.";
        }
        public override void Update()
        {
            base.Update();
        }
    }
}
