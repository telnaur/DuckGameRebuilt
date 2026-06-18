using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Magic")]
    public class OstrichGun : Gun
    {

        public OstrichGun(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "OstrichGun";
            this.ammo = 99999;
            this.graphic = new Sprite(this.GetPath("OstrichGun"), 0.0f, 0.0f);
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -4f);
            this.collisionSize = new Vec2(16f, 9f);
            this._barrelOffsetTL = new Vec2(27f, 14f);
            this._fireSound = "SFX/lob";
            this._kickForce = 2f;
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                --this.ammo;
                SFX.Play("crateHit", 1f, 0.0f, 0.0f, false);
                this.ApplyKick();
                Vec2 vec2 = this.Offset(this.barrelOffset);
                if (this.receivingPress)
                    return;
                Present present = new Present(vec2.x, vec2.y - 2f);
                Level.Add((Thing)present);
                this.Fondle((Thing)present);
                if (this.owner != null)
                    present.responsibleProfile = this.owner.responsibleProfile;
                present.clip.Add(this.owner as MaterialThing);
                present.hSpeed = this.barrelVector.x * 4f;
                present.vSpeed = this.barrelVector.y * 3f;






            }
            else
                this.DoAmmoClick();
        }






        public override void Fire()
        {
        }
    }
}