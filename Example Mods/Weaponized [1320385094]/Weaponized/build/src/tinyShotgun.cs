using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Shotguns")]
    public class tinyShotgun : Gun
    {
        public tinyShotgun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = (AmmoType)new tinyATShrapnel();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("tinyShotgun"), 0.0f, 0.0f);
            this.center = new Vec2(6f, 5f);
            this.collisionOffset = new Vec2(-6f, -5f);
            this.collisionSize = new Vec2(12f, 10f);
            this._barrelOffsetTL = new Vec2(12f, 3f);
            this._fireSound = "shotgunFire";
            this._kickForce = 3f;
            this._numBulletsPerFire = 4;
            this.ammoType.rebound = false;
            this.ammoType.rangeVariation = 10f;
            this._fireWait = 2f;
            this._editorName = "Tiny Shotgun";
            this.editorTooltip = "It makes up for its short range with 4 quick shots.";
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
