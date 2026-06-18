using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | FireAndWater")]
    class Fireblowing : Gun
    {
        public Fireblowing(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Fireblowing";
            this.ammo = 80;
            this._ammoType = new ATMin();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("Fireblowing"), 32, 14);
            this.center = new Vec2(12.5f, 10f);
            this._holdOffset = new Vec2(0f, 0f);
            this._fullAuto = true;
            this._fireWait = 0;
            this.heat = 0f;
            this._fireSound = GetPath("SFX/firewandnoise");
            this.collisionOffset = new Vec2(-16f, -7f);
            this.collisionSize = new Vec2(32f, 14f);
            this._barrelOffsetTL = new Vec2(32f, 6f);

        }
        public override void OnHoldAction()
        {
            if (this.ammo > 0)
            {
                --this.ammo;
                this.kick = 1f;
                if (this.receivingPress || !this.isServerForObject)
                    return;
                Vec2 vec = Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f));
                Vec2 vec2 = new Vec2(vec.x * Rando.Float(8f, 10f), vec.y);
                Level.Add((Thing)SmallFire.New(this.barrelPosition.x, this.barrelPosition.y, vec2.x, vec2.y, false, (MaterialThing)null, true, (Thing)this, false));
                SFX.Play(_fireSound, 1f, Rando.Float(0.2f) - 0.1f, 0f, false);
            }
        }
        public override void Fire() { 
        }

    }
}

