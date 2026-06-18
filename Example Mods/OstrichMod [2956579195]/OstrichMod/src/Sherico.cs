using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Eternal")]
    class Sherico : Gun
    {
        private SpriteMap sprite;

        public Sherico(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Sherico";
            this.ammo = 6;
            this._ammoType = new ATGrenade();
            this._type = "gun";
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Sherico"),24, 14);
            base.graphic = this.sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-15f, -4f);
            this.collisionSize = new Vec2(24f, 10f);
            this._barrelOffsetTL = new Vec2(32f, 2f);
            this._kickForce = 0.5f;
            this._fireSound = GetPath("SFX/buzzsawNoise");
            this.weight = 5f;
            this._editorName = "Sherico";
            this._ammoType.bulletSpeed = 100f;
            this._ammoType.range = 300f;
        }

    }
}
