using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Tech")]
    class Trabaco : Gun
    {
        private SpriteMap sprite;

        public Trabaco(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Trabaco";
            this.ammo = 5;
            this._ammoType = new ATMag();
            this._type = "gun";
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Trabaco"), 18, 10);
            base.graphic = this.sprite;
            this.center = new Vec2(8f, 4f);
            this.collisionOffset = new Vec2(-8f, 0f);
            this.collisionSize = new Vec2(18f, 10f);
            this._barrelOffsetTL = new Vec2(18f, 3f);
            this._holdOffset = new Vec2(-1f, -1f);
            this._fireSound = GetPath("sounds/revolver");
            this.weight = 5f;
            this._numBulletsPerFire = 5;
            this._bulletColor = Color.Black;
            this._ammoType.range = 15f;
            this._ammoType.accuracy = 0.1f;
        }

    }
}
