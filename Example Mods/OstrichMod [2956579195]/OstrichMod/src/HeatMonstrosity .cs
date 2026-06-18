using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Virgo")]
    class HeatMonstrosity : Gun
    {
        private SpriteMap sprite;

        public HeatMonstrosity(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Heat Monstrosity";
            this.ammo = 20000;
            this._ammoType = (AmmoType)new ATRedLacer();
            this._fullAuto = true;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("HeatMonstrosity"),34, 10);
            base.graphic = this.sprite;
            this.center = new Vec2(9f, 5f);
            this.collisionOffset = new Vec2(-9f, -5f);
            this.collisionSize = new Vec2(34f, 10f);
            this._barrelOffsetTL = new Vec2(34f, 4f);
            this._kickForce = 1f;
            this._holdOffset = new Vec2(0f, 0.0f);
            this._fireSound = GetPath("SFX/pewpew");
            this._ammoType.affectedByGravity = false;
            this._fireWait = 3f;
            this._ammoType.accuracy = 0.63f;
        }
        public override void Fire()
        {
            this.heat += 0.0035f;
            base.Fire();
        }
        public override void OnHoldAction()
        {
            if (this._fireWait > 0)
            {
                this._fireWait -= 0.01f;
            }
            base.OnHoldAction();
        }
        public override void OnReleaseAction()
        {
            this._fireWait = 3f;
            base.OnReleaseAction();
        }

    }
}
