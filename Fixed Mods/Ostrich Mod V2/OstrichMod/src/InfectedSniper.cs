using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | BioLogic")]
    class InfectedSniper : Gun
    {
        public InfectedSniper(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "InfectedSniper";
            this.ammo = 6;
            this._ammoType = new ATMin();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("InfectedSniper"),36, 10);
            this.center = new Vec2(13f, 5f);
            this.collisionOffset = new Vec2(-13f, -5f);
            this.collisionSize = new Vec2(36f, 10f);
            this._barrelOffsetTL = new Vec2(36f, 4f);
            this._holdOffset = new Vec2(-6f, -1f);
            this._fullAuto = false;
            this._fireWait = 3;
            this._fireSound = GetPath("SFX/Parasite");
            this.laserSight = true;
            this._laserOffsetTL = new Vec2(36f, 4f);
        }
        public override void OnPressAction()
        {
            if(this.ammo > 0)
            {
                --this.ammo;
                this.kick = 1f;
                if (this.receivingPress || !this.isServerForObject)
                    return;
                Vec2 vec2_1 = this.Offset(this.barrelOffset);
                float radians = this.barrelAngle + Rando.Float(0f, 0f);
                ATParasite rail = new ATParasite(vec2_1.x, vec2_1.y, this.owner as Duck, 0);
                this.Fondle((Thing)rail);
                Vec2 vec2_2 = Maths.AngleToVec(radians);
                rail.hSpeed = vec2_2.x * 1000f;
                rail.vSpeed = vec2_2.y * 1000f;
                Level.Add((Thing)rail);
                SFX.Play(_fireSound, 1f, Rando.Float(0.2f) - 0.1f, 0f, false);
            }
        }

    }
}