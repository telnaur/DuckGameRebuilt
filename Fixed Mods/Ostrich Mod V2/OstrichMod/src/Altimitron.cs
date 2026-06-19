using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Tech")]
    class Altimitron : Gun
    {
        private SpriteMap sprite;

        public Altimitron(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Altimitron";
            this.ammo = 99;
            this._ammoType = (AmmoType)new ATMin();
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Altimitron"),30, 12);
            base.graphic = this.sprite;
            this.center = new Vec2(17f, 5f);
            this.collisionOffset = new Vec2(-17f, -5f);
            this.collisionSize = new Vec2(30f, 12f);
            this._barrelOffsetTL = new Vec2(29f, 5f);
            this._kickForce = 0f;
            this._holdOffset = new Vec2(0f, -3f);
            this._fireSound = GetPath("sounds/electricidad");
            this._fireWait = 0f;
            this._editorName = "Altimitron";
        }

    }
}
