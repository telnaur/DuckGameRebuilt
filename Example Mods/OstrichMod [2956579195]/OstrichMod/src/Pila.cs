using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Tech")]
    class Pila : Gun
    {
        private SpriteMap sprite;

        public Pila(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Pila";
            this.ammo = 999;
            this._ammoType = (AmmoType)new ATrayos();
            this._fullAuto = true;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Pila"),21, 11);
            base.graphic = this.sprite;
            this.center = new Vec2(8f, 5f);
            this.collisionOffset = new Vec2(-8f, -5f);
            this.collisionSize = new Vec2(21f, 11f);
            this._barrelOffsetTL = new Vec2(19f, 5f);
            this._kickForce = 0f;
            this._holdOffset = new Vec2(0f, -2f);
            this._fireSound = GetPath("sounds/electricidad");
            this._fireWait = 0f;
            this._editorName = "Pila";
        }

    }
}
