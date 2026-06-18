using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Machine Guns")]
    [BaggedProperty("isSuperWeapon", true)]
    public class scar : Gun
    {
        public scar(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 20;
            this._ammoType = (AmmoType)new ATHighCalMachinegun();
            this._ammoType.range = 320f;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("scar"), 0.0f, 0.0f);
            this.center = new Vec2(16f, 17f);
            this.collisionOffset = new Vec2(-13f, -6f);
            this.collisionSize = new Vec2(26f, 11f);
            this._barrelOffsetTL = new Vec2(32f, 13f);
            this._fireSound = "deepMachineGun2";
            this._fullAuto = true;
            this._fireWait = 2f;
            this._kickForce = 3f;
            this.loseAccuracy = 0.25f;
            this.maxAccuracyLost = 0.50f;
            this._holdOffset = new Vec2(0f, 2f);
            this._editorName = "FN SCAR";
            this.editorTooltip = "AK's older brother, slower but more accurate.";
        }
    }
}
