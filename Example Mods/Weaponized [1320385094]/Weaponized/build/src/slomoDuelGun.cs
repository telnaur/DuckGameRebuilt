using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{

    [EditorGroup("Zyrafa|Guns|Slo-Mo")]
    public class slomoDuelGun : Gun
    {
        public slomoDuelGun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new slomoATDuel();
            this._ammoType.range = 4000f;
            this._ammoType.accuracy = 0.7f;
            this._ammoType.penetration = 0.4f;
            this._ammoType.immediatelyDeadly = true;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("slomoDuelGun"), 0.0f, 0.0f);
            this.center = new Vec2(17f, 16f);
            this.collisionOffset = new Vec2(-7f, -4f);
            this.collisionSize = new Vec2(17f, 9f);
            this._barrelOffsetTL = new Vec2(25f, 15f);
            this._fireSound = "littleGun";
            this._kickForce = 4f;
            this._editorName = "Slo-Mo Dueling Gun";
            this.editorTooltip = "These bullets are so slooow!";
        }
    }
}
