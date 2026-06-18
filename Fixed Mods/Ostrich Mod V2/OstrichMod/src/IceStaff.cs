using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Magic")]
    class IceStaff : Gun
    {
        public IceStaff(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Ice Staff";
            this.ammo = 120;
            this._ammoType = new ATBlueLacer();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("IceStaff"),11, 30);
            this.center = new Vec2(5f, 15f);
            this.collisionOffset = new Vec2(-5f, -15f);
            this.collisionSize = new Vec2(11f, 30f);
            this._barrelOffsetTL = new Vec2(5f, 0f);
            this._holdOffset = new Vec2(-4f, 0f);
            this._fullAuto = true;
            this._kickForce = 0f;
            this.loseAccuracy = 0f;
            this.maxAccuracyLost = 0.0f;
            this._ammoType.accuracy = 306f;
            this._fireWait = 0.5f;
            this._fireSound = GetPath("SFX/drillklang");
        }
    }
}