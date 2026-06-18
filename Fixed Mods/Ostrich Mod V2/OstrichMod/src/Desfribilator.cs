using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | ELC")]
    class Desfribilator : Gun
    {
        private SpriteMap sprite;

        public Desfribilator(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Desfribilator";
            this.ammo = 8;
            this._ammoType = (AmmoType)new AT9mm();
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Desfribilator"),6, 12);
            base.graphic = this.sprite;
            this.center = new Vec2(2f, 7f);
            this.collisionOffset = new Vec2(2f, -6f);
            this.collisionSize = new Vec2(6f, 10f);
            this._barrelOffsetTL = new Vec2(0f, 0f);
            this._kickForce = 0f;
            this._holdOffset = new Vec2(0f, 0.0f);
            this._fireSound = GetPath("SFX/pewpew");
            this._fireWait = 0.5f;
        }
        public override void Fire()
        {
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                this.ammo--;
                if (this.isServerForObject)
                {
                    Level.Add((Thing)new ElectricalCharge(this.barrelPosition.x, this.barrelPosition.y + 8, (int)this.offDir, (Thing)this));
                }
            }
            base.OnPressAction();
        }
    }
}

