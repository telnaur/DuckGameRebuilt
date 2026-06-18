using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | ELC")]
    class Goiba : Gun
    {
        private SpriteMap sprite;

        public Goiba(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Goiba";
            this.ammo = 60;
            this._ammoType = (AmmoType)new ATrayos();
            this._fullAuto = true;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Goiba"),54, 20);
            base.graphic = this.sprite;
            this.center = new Vec2(24f, 10f);
            this.collisionOffset = new Vec2(-9f, -5f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(34f, 6f);
            this._ammoType.accuracy = 0.2f;
            this._kickForce = 0f;
            this._holdOffset = new Vec2(4f, 0.0f);
            this._numBulletsPerFire = 3;
            this._fireSound = GetPath("sounds/electricidad");
            this._fireWait = 1f;
            this._editorName = "Goiba";
        }

    }
}
