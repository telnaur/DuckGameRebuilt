using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Pistols")]
    public class badMagnum : Gun
    {
        public StateBinding _angleOffsetBinding = new StateBinding("_angleOffset", -1, false, false);
        public StateBinding _riseBinding = new StateBinding("rise", -1, false, false);
        public float rise;
        public float _angleOffset;

        public override float angle
        {
            get
            {
                return base.angle + this._angleOffset;
            }
            set
            {
                this._angle = value;
            }
        }

        public badMagnum(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 6;
            this._ammoType = (AmmoType)new ATMagnum();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("badMagnum"), 0.0f, 0.0f);
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -6f);
            this.collisionSize = new Vec2(18f, 11f);
            this._barrelOffsetTL = new Vec2(25f, 13f);
            this._fireSound = "magnum";
            this._kickForce = 3.5f;
            this._holdOffset = new Vec2(1f, 2f);
            this.handOffset = new Vec2(0.0f, 1f);
            this.ammoType.accuracy = 0.12f;
            this._editorName = "Old Magnum";
            this.editorTooltip = "This revolver has seen better days, please excuse its inaccuracy.";
        }

        public override void Update()
        {
            base.Update();
            this._angleOffset = this.owner == null ? 0.0f : ((int)this.offDir >= 0 ? -Maths.DegToRad(this.rise * 65f) : -Maths.DegToRad((float)(-(double)this.rise * 65.0)));
            if ((double)this.rise > 0.0)
                this.rise -= 0.013f;
            else
                this.rise = 0.0f;
            if (!this._raised)
                return;
            this._angleOffset = 0.0f;
        }

        public override void OnPressAction()
        {
            base.OnPressAction();
            if (this.ammo <= 0 || (double)this.rise >= 1.0)
                return;
            this.rise += 0.4f;
        }
    }
}
