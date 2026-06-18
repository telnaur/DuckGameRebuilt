using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Tech")]
    class Castra : Gun
    {
        private SpriteMap sprite;

        public Castra(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Castra";
            this.ammo = 3;
            this._ammoType = new ATMissile();
            this._type = "gun";
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Castra"), 28, 10);
            base.graphic = this.sprite;
            this.center = new Vec2(14f, 4f);
            this.collisionOffset = new Vec2(-14f, -4f);
            this.collisionSize = new Vec2(28f, 10f);
            this._barrelOffsetTL = new Vec2(30f, 4f);
            this._holdOffset = new Vec2(3f, -2f);
            this._fireSound = GetPath("SFX/iceblockHit");
            this.weight = 5f;
            this._numBulletsPerFire = 10;
            this._bulletColor = Color.Red;
            this._ammoType.bulletSpeed = 100f;
            this._editorName = "Castra";
        }

    }
}
