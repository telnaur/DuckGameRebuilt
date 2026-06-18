using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | FireAndWater")]
    class Bananator : Gun
    {
        public Bananator(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Bananator";
            this.ammo = 32;
            this._kickForce = 0.3f;
            this._ammoType = new ATMin();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("Bananator"), 21, 13);
            this.center = new Vec2(7f, 8f);
            this._fullAuto = true;
            this._fireWait = 0.5f;
            this._fireSound = GetPath("Sounds/Coconut");
            this.collisionOffset = new Vec2(-7f, -8f);
            this.collisionSize = new Vec2(21f, 13f);
            this._barrelOffsetTL = new Vec2(22f, 4f);

        }
        public override void Fire() 
        {
            if (this.ammo > 0 && _wait <= 0)
            {
                --this.ammo;
                this.kick = 1f;
                if (this.receivingPress || !this.isServerForObject)
                    return;

                Vec2 vec2_1 = this.Offset(this.barrelOffset);
                float radians = this.barrelAngle + Rando.Float(0f, 0f);
                Banana banana = new Banana(vec2_1.x, vec2_1.y);
                Vec2 vec2_2 = Maths.AngleToVec(radians);
                banana.hSpeed = vec2_2.x * (10f * Rando.Float(0.5f, 1f));
                banana.vSpeed = vec2_2.y * Rando.Float(-1f, 1f);
                banana.PressAction();
                Level.Add(banana);
                this._wait = this._fireWait;

                SFX.Play(_fireSound, 1f, Rando.Float(0.2f) - 0.1f, 0f, false);
            }
        }

    }
}

