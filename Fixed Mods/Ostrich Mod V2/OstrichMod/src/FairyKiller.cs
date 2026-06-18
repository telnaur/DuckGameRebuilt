using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | BioLogic")]
    class FairyKiller : Gun
    {
        public FairyKiller(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "FairyKiller";
            this.ammo = 12;
            this._ammoType = new ATMin();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("FairyKiller"),28, 12);
            this.center = new Vec2(11f, 7f);
            this.collisionOffset = new Vec2(-11f, -7f);
            this.collisionSize = new Vec2(28f, 12f);
            this._barrelOffsetTL = new Vec2(48f, 4f);
            this._holdOffset = new Vec2(0f, 0f);
            this._fullAuto = false;
            this._fireWait = 0;
            this.flammable = 1f;
            this._fireSound = GetPath("SFX/Parasite");
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
                Level.Add(new ToxicSmoke(vec2_1.x, vec2_1.y, 3f + Rando.Float(1f)));
                SFX.Play(_fireSound, 1f, Rando.Float(0.2f) - 0.1f, 0f, false);
            }
        }

    }
}