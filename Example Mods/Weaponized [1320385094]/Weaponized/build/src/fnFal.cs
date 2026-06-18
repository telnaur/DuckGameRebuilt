using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Machine Guns")]
    [BaggedProperty("isSuperWeapon", true)]
    public class fnFal : Gun
    {
        public fnFal(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 20;
            this._ammoType = (AmmoType)new ATHighCalMachinegun();
            this._ammoType.range = 240f;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("fnFal"), 0.0f, 0.0f);
            this.center = new Vec2(17f, 6f);
            this.collisionOffset = new Vec2(-19f, -6f);
            this.collisionSize = new Vec2(37f, 12f);
            this._barrelOffsetTL = new Vec2(37f, 4f);
            this._fireSound = "deepMachineGun2";
            this._fullAuto = true;
            this._fireWait = 1.5f;
            this._kickForce = 2.5f;
            this._fireSoundPitch = 1.2f;
            this.loseAccuracy = 0.2f;
            this.maxAccuracyLost = 0.7f;
            this._holdOffset = new Vec2(1f, 1f);
            this._editorName = "FN FAL";
            this.editorTooltip = "A fine balance between AK's rate of fire and SCAR's accuracy.";
        }
    }
}
