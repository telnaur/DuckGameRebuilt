using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Eternal")]
    class Aeres : Gun
    {
        private SpriteMap sprite;

        public Aeres(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Aeres";
            this.ammo = 5;
            this._ammoType = new ATAeres();
            this._type = "gun";
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Kuda"),26, 12);
            base.graphic = this.sprite;
            this.center = new Vec2(14f, 5f);
            this.collisionOffset = new Vec2(-13f, -6f);
            this.collisionSize = new Vec2(26f, 12f);
            this._barrelOffsetTL = new Vec2(26f, 6f);
            this._kickForce = 1f;
            this._holdOffset = new Vec2(4f, -2f);
            this._fireSound = GetPath("SFX/buzzsawNoise");
            this.weight = 5f;
            this._editorName = "Aeres";
        }

    }
}
