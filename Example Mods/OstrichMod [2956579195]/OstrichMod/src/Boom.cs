using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | ELC")]
    class Boom : Gun
    {
        private SpriteMap sprite;

        public Boom(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Boom";
            this.ammo = 5;
            this._ammoType = (AmmoType)new AT9mm();
            this._ammoType.combustable = true;
            this._fullAuto = true;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Boom"),6, 12);
            base.graphic = this.sprite;
            this.center = new Vec2(2f, 7f);
            this.collisionOffset = new Vec2(-2f, -5f);
            this.collisionSize = new Vec2(6f, 10f);
            this._barrelOffsetTL = new Vec2(150f, 0f);
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
                    GrenadeExplosion exp = new GrenadeExplosion(0, 0);
                    exp.position = Offset(this._barrelOffsetTL);
                    exp.hSpeed = this.barrelVector.x * 7f;
                    exp.vSpeed = this.barrelVector.y * 7f;
                    Level.Add((Thing)exp);
                }
            }
            base.OnPressAction();
        }
    }
}

