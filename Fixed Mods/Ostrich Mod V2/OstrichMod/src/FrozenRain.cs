using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | FireAndWater")]
    class FrozenRain : Gun
    {
        private SpriteMap sprite;

        public FrozenRain(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "FrozenRain";
            this.ammo = 15;
            this._ammoType = new ATBlueLacer();
            this._type = "gun";
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("FrozenRain"),37, 12);
            base.graphic = this.sprite;
            this.center = new Vec2(15f, 7f);
            this.collisionOffset = new Vec2(-15f, -7f);
            this.collisionSize = new Vec2(36f, 12f);
            this._barrelOffsetTL = new Vec2(30f, 2f);
            this._holdOffset = new Vec2(0f, -1f);
            this._fireSound = GetPath("sounds/bow");
            this._numBulletsPerFire = 5;
            this._ammoType.accuracy = 0.3f;
        }

    }
}
