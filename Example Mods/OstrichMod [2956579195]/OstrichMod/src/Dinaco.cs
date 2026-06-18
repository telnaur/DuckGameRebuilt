using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Virgo")]
    class Dinaco : Gun
    {
        private SpriteMap sprite;

        public Dinaco(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Dinaco";
            this.ammo = 200;
            this._ammoType = (AmmoType)new ATLaser();
            this._fullAuto = true;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Dinaco"),17, 13);
            base.graphic = this.sprite;
            this.center = new Vec2(9f, 6f);
            this.collisionOffset = new Vec2(-9f, -5f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(16f, 3f);
            this._kickForce = 0f;
            this._holdOffset = new Vec2(4f, 0.0f);
            this._fireSound = GetPath("SFX/pewpew");
            this._fireWait = 0.5f;
            this._editorName = "Dinaco";
        }

    }
}
