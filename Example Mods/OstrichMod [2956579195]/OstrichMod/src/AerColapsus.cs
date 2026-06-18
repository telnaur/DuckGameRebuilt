using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;


namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Tech")]
    class AerColapsus : Gun
    {
        private SpriteMap sprite;

        public AerColapsus(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "AerColapsus";
            this.ammo = 4;
            this._ammoType = new ATLaser();
            this._type = "gun";
            this._fullAuto = false;
            this.sprite = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("AerColapsus"),26, 13);
            base.graphic = this.sprite;
            this.center = new Vec2(15f, 5f);
            this.collisionOffset = new Vec2(-15f, -5f);
            this.collisionSize = new Vec2(26f, 13f);
            this._barrelOffsetTL = new Vec2(27f, 0f);
            this._holdOffset = new Vec2(5f, -4f);
            this._fireSound = GetPath("SFX/beepbeep");
            this._kickForce = 2f;
            this.weight = 5f;
            this._editorName = "Aer Colapsus";
        }
        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                --this.ammo;
                this.kick = 1f;
                if (this.receivingPress || !this.isServerForObject)
                    return;
                Vec2 vec2_1 = this.Offset(this.barrelOffset);
                float radians = this.barrelAngle + Rando.Float(0f, 0f);
                AirMine rail = new AirMine(vec2_1.x, vec2_1.y, this.owner as Duck, 0);
                this.Fondle((Thing)rail);
                Vec2 vec2_2 = Maths.AngleToVec(radians);
                rail.hSpeed = vec2_2.x * 4f;
                rail.vSpeed = vec2_2.y * 4f;
                Level.Add((Thing)rail);
                SFX.Play(_fireSound, 1f, Rando.Float(0.2f) - 0.1f, 0f, false);
            }
        }
    }
}
