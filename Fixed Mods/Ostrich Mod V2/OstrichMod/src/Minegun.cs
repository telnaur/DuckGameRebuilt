using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Tech")]
    public class Minegun : Gun
    {

        public Minegun(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Minegun";
            this.ammo = 6;
            this.graphic = new Sprite(this.GetPath("Minegun"), 22f, 9f);
            this.center = new Vec2(12f, 4f);
            this.collisionOffset = new Vec2(-12f, -3f);
            this.collisionSize = new Vec2(22f, 8f);
            this._barrelOffsetTL = new Vec2(23f, 4f);
            this._holdOffset = new Vec2(3f, 0f);
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
                Mine mine = new Mine(vec2.x, vec2.y - 2f);
                Level.Add((Thing)mine);
                this.Fondle((Thing)mine);
                if (this.owner != null)
                    mine.responsibleProfile = this.owner.responsibleProfile;
                mine._pin = false;
                mine.clip.Add(this.owner as MaterialThing);
                mine.hSpeed = this.barrelVector.x * 5f;
                mine.vSpeed = this.barrelVector.y * 3f;






            }
            else
                this.DoAmmoClick();
        }






        public override void Fire()
        {
        }
    }
}