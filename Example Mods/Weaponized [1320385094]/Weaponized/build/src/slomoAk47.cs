using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{

    [EditorGroup("Zyrafa|Guns|Slo-Mo")]
    public class slomoAK47 : Gun
    {
        public slomoAK47(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 30;
            this._ammoType = (AmmoType)new slomoAT9mm();
            this._ammoType.range = 280f;
            this._ammoType.accuracy = 0.85f;
            this._ammoType.penetration = 2f;
            this._ammoType.immediatelyDeadly = true;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("slomoAk47"), 0.0f, 0.0f);
            this.center = new Vec2(16f, 15f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(18f, 10f);
            this._barrelOffsetTL = new Vec2(32f, 14f);
            this._fireSound = "deepMachineGun2";
            this._fullAuto = true;
            this._fireWait = 1.2f;
            this._kickForce = 3.5f;
            this.loseAccuracy = 0.2f;
            this.maxAccuracyLost = 0.8f;
            this._editorName = "Slo-Mo AK47";
            this.editorTooltip = "These bullets are so slooow!";
        }
    }
}
