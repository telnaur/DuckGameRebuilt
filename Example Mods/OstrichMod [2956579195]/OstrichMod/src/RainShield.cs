using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Virgo")]
    class RainShield : Gun
    {
        private SpriteMap sprite;

        public RainShield(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "RainShield";
            this.ammo = 200;
            this._ammoType = (AmmoType)new ATShrapnel();
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Shield"),19, 19);
            base.graphic = this.sprite;
            this.center = new Vec2(2f, 9f);
            this.collisionOffset = new Vec2(-2f, -9f);
            this.collisionSize = new Vec2(19f, 19f);
            this._barrelOffsetTL = new Vec2(18f, 9f);
            this._kickForce = 0f;
            this._holdOffset = new Vec2(0f, 0.0f);
            this._fireSound = GetPath("SFX/bowShotSFX");
            this._fireWait = 0.5f;
            this.loseAccuracy = 0.4f;
            this.maxAccuracyLost = 0.8f;
            this._ammoType.range = 120f;
            physicsMaterial = PhysicsMaterial.Metal;
            depth = -0.5f;
            thickness = 1.2f;
            weight = 2f;
        }

    }
}
