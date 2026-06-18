using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Magic")]
    class Mortix : Gun
    {
        public Mortix(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Mortix";
            this.ammo = 120;
            this._ammoType = new ATMortix();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("Mortix"),13, 29);
            this.center = new Vec2(6f, 16f);
            this.collisionOffset = new Vec2(-6f, -16f);
            this.collisionSize = new Vec2(13f, 29f);
            this._barrelOffsetTL = new Vec2(6f, 1f);
            this._holdOffset = new Vec2(-4f, 0f);
            this._fullAuto = true;
            this._kickForce = 0f;
            this.loseAccuracy = 5f;
            this.maxAccuracyLost = 0f;
            this._ammoType.accuracy = 4f;
            this._numBulletsPerFire = 5;
            this._fireWait = 0;
            this._fireSound = GetPath("SFX/drillklang");
        }
    }
}