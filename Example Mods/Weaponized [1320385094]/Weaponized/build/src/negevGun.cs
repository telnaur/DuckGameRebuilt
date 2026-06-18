using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Machine Guns")]
    public class negevGun : Gun
    {
        public StateBinding _angleOffsetBinding = new StateBinding("_angleOffset", -1, false);
        public StateBinding _riseBinding = new StateBinding("rise", -1, false);
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
        private SpriteMap _sprite;
        public negevGun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 75;
            this._ammoType = (AmmoType)new AT9mm();
            this._ammoType.range = 150f;
            this._ammoType.accuracy = 0.80f;
            this._ammoType.penetration = 1f;
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("negev"), 30, 11, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(15f, 6f);
            this.collisionOffset = new Vec2(-15f, -6f);
            this.collisionSize = new Vec2(30f, 11f);
            this._barrelOffsetTL = new Vec2(30f, 4f);
            this._fireSound = "pistolFire";
            //this._fireSound = "shotgunFire2";
            this._fullAuto = true;
            this._fireWait = 0.4f;
            this._kickForce = 0.5f;
            this.loseAccuracy = 0.1f;
            this.maxAccuracyLost = 0.4f;
            this._sprite.AddAnimation("rest", 0.2f, true, 0);
            this._sprite.AddAnimation("shooting", 0.2f, true, 1, 0);
            this._sprite.AddAnimation("empty", 0.2f, true, 2);
            this._sprite.SetAnimation("rest");
            this.weight = 6.5f;
            this._holdOffset = new Vec2(0f, 2f);
            this._editorName = "Negev";
            this.editorTooltip = "Lots of bullets but the recoil might be too much to handle.";
        }
       public override void Update()
        {
            base.Update();
            if (this.ammo <= 0)
                this._sprite.SetAnimation("empty");
            this._angleOffset = this.owner == null ? 0.0f : (this.offDir >= (sbyte)0 ? -Maths.DegToRad(this.rise * 65f) : -Maths.DegToRad((float)(-(double)this.rise * 65.0)));
            if ((double)this.rise > 0.0)
                this.rise -= 0.013f;
            else
                this.rise = 0.0f;
            if (!this._raised)
                return;
            this._angleOffset = 0.0f;

        }
        public override void OnReleaseAction()
        {
            if (this.ammo > 0)
                this._sprite.SetAnimation("rest");
            base.OnReleaseAction();
        }
        public override void OnPressAction()
        {
            base.OnPressAction();
            if (this.ammo > 0)
                this._sprite.SetAnimation("shooting");

        }
        public override void OnHoldAction()
        {
            if (this.ammo <= 0 || (double)this.rise >= 1.0)
                return;
            this.rise += 0.08f;
            base.OnHoldAction();
        }
    }
}
