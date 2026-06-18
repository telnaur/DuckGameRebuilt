using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Magic")]
    class SolumStaff : Gun
    {
        public SolumStaff(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Solum Staff";
            this.ammo = 1;
            this._ammoType = new ATForce();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("SolumStaff"),11, 28);
            this.center = new Vec2(5f, 17f);
            this.collisionOffset = new Vec2(-5f, -17f);
            this.collisionSize = new Vec2(11f, 28f);
            this._barrelOffsetTL = new Vec2(5f, 4f);
            this._holdOffset = new Vec2(-4f, 0f);
            this._fullAuto = false;
            this._kickForce = 0f;
            this.loseAccuracy = 0f;
            this.maxAccuracyLost = 0.0f;
            this._ammoType.accuracy = 360f;
            this._numBulletsPerFire = 30;
            this._fireWait = 0;
            this._fireSound = GetPath("SFX/drillklang");
        }
    }
}