using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Virgo")]
    class Playboy : Gun
    {
        private SpriteMap sprite;

        public Playboy(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Playboy";
            this.ammo = 10;
            this._ammoType = (AmmoType)new ATPene();
            this._fullAuto = true;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Playboy"),54, 20);
            base.graphic = this.sprite;
            this.center = new Vec2(24f, 10f);
            this.collisionOffset = new Vec2(-9f, -5f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(30f, 10f);
            this._kickForce = 3f;
            this._holdOffset = new Vec2(4f, 0.0f);
            this._fireSound = GetPath("sounds/electricidad");
            this._fireWait = 0f;
            this._editorName = "Playboy";
        }

    }
}
