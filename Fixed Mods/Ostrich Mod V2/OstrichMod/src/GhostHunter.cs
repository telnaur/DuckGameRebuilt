using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Eternal")]
    class GhostHunter : Gun
    {
        private SpriteMap sprite;

        public GhostHunter(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "GhostHunter";
            this.ammo = 8;
            this._ammoType = new ATMagnum();
            this._type = "gun";
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("GhostHunter"),25, 14);
            base.graphic = this.sprite;
            this.center = new Vec2(13f, 5f);
            this.ammoType.speedVariation = 0.0f;
            this._ammoType.accuracy = 0.5f;
            this.ammoType.bulletSpeed = 2f;
            this.collisionOffset = new Vec2(-15f, -4f);
            this.collisionSize = new Vec2(25f, 15f);
            this._barrelOffsetTL = new Vec2(32f, 2f);
            this._holdOffset = new Vec2(4f, 0.0f);
            this._kickForce = 2f;
            this._numBulletsPerFire = 5;
            this._fireSound = GetPath("sounds/Curse");
            this.weight = 5f;
            this._editorName = "Ghost Hunter";
        }

    }
}
