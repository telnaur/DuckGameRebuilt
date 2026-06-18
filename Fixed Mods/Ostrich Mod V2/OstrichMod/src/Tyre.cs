using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | ELC")]
    class Tyre : Gun
    {
        private SpriteMap sprite;

        public Tyre(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Tyre";
            this.ammo = 3;
            this._ammoType = (AmmoType)new ATMortal();
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Tyre"),25, 16);
            base.graphic = this.sprite;
            this.center = new Vec2(22f, 10f);
            this.collisionOffset = new Vec2(-9f, -5f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(30f, 10f);
            this._kickForce = 0f;
            this._holdOffset = new Vec2(4f, 0.0f);
            this._fireSound = GetPath("SFX/meting");
            this._fireWait = 1f;
            this._editorName = "Tyre";   
            this.laserSight = true;
	    this._laserOffsetTL = new Vec2(22f, 10f);
           
        }

    }
}
