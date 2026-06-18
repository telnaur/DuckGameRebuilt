using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | BioLogic")]
    class Krikion : Gun
    {
        public Krikion(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Krikion";
            this.ammo = 6;
            this._ammoType = new ATMin();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("Krikion2"), 22, 15);
            this.center = new Vec2(11f, 6f);
            this.collisionOffset = new Vec2(-11f, -6f);
            this.collisionSize = new Vec2(22f, 15f);
            this._barrelOffsetTL = new Vec2(5f, 0f);
            this._holdOffset = new Vec2(4f, -4f);
            this._fullAuto = false;
            this._kickForce = 2f;
            this._fireWait = 0.5f;
            this._fireSound = GetPath("SFX/iceblockHit");
        }
        public override void OnPressAction()
        {
            this.graphic = new Sprite(GetPath("Krikion1"), 22, 15);
            if (this.ammo <= 0)
                return;
            this.ammo--;
            ATMucosa exp = new ATMucosa(0, 0);
            exp.position = Offset(this._barrelOffsetTL);
            exp.hSpeed = this.barrelVector.x * 14f;
            exp.vSpeed = this.barrelVector.y * 14f;
            Level.Add((Thing)exp);
            SFX.Play(GetPath("SFX/iceblockHit"), 1f, 0.0f, 0.0f, false);
        }
        public override void OnReleaseAction()
        {
            this.graphic = new Sprite(GetPath("Krikion2"), 22, 15);
        }

    }
}
