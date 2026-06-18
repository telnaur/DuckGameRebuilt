using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Eternal")]
    public class Impacto : Gun
    {

        public Impacto(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Impacto";
            this.ammo = 60;
            this.graphic = new Sprite(this.GetPath("Impacto"), 26f, 14f);
            this.center = new Vec2(12f, 5f);
            this.collisionOffset = new Vec2(-12f, -5f);
            this.collisionSize = new Vec2(26f, 14f);
            this._barrelOffsetTL = new Vec2(28f, -4f);
            this._holdOffset = new Vec2(4f, -3f);
            this._fireSound = "SFX/lob";
            this._kickForce = 6f;
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                --this.ammo;
                SFX.Play("crateHit", 1f, 0.0f, 0.0f, false);
                this.ApplyKick();
                if (base.isServerForObject)
                {
                    Level.Add(new ForceWave(base.x + (float)offDir * 4f + owner.hSpeed, base.y + 8f, offDir, 0.15f, 12f + Math.Abs(owner.hSpeed), owner.vSpeed, base.duck));

                }
                
            }
            else
                this.DoAmmoClick();
        }


        public override void Fire()
        {
        }
    }
}