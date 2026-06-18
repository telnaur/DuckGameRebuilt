using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [BaggedProperty("isFatal", false)]
    [EditorGroup("Zyrafa|Guns|Pistols")]
    public class DuelingMagnum : Gun
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
        public DuelingMagnum(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new ATMagnum();
            this._ammoType.range = 150f;
            this._ammoType.accuracy = 0.8f;
            this._ammoType.penetration = 2f;
            this.ammoType.bulletSpeed = 36f;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("duelingMagnum"), 0.0f, 0.0f);
            this.center = new Vec2(19f, 16f);
            this.collisionOffset = new Vec2(-9f, -4f);
            this.collisionSize = new Vec2(19f, 8f);
            this._barrelOffsetTL = new Vec2(27f, 15f);
            this._fireSound = "littleGun";
            this._kickForce = 3f;
            this._holdOffset = new Vec2(3f, 0f);
            this._editorName = "Dueling Magnum";
            this.editorTooltip = "The magnum's power with the dueling gun's ammo count.";

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
