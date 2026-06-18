using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Tech")]
    internal class Shuder : Gun
    {
        public StateBinding _fireAngleState = new StateBinding("_fireAngle", -1, false);
        public StateBinding _aimAngleState = new StateBinding("_aimAngle", -1, false);
        public StateBinding _aimWaitState = new StateBinding("_aimWait", -1, false);
        public StateBinding _aimingState = new StateBinding("_aiming", -1, false);
        public StateBinding _cooldownState = new StateBinding("_cooldown", -1, false);
        public float _fireAngle;
        public float _aimAngle;
        public float _aimWait;
        public bool _aiming;
        public float _cooldown;

        public override float angle
        {
            get
            {
                return base.angle + this._aimAngle;
            }
            set
            {
                this._angle = value;
            }
        }
        public Shuder(float xval, float yval)
      : base(xval, yval)
        {
            this._editorName = "Shuder";
            this.ammo = 20;
            this._ammoType = new ATReboundLaser();
            this._fireWait = 0.01f;
            this._ammoType.range = 1000000f;
            this.graphic = new Sprite(GetPath("Shuder"));
            this.center = new Vec2(24f, 10f);
            this.collisionOffset = new Vec2(-9f, -5f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(30f, 10f);
            this._kickForce = 2f;
            this._holdOffset = new Vec2(4f, 0.0f);
            this._weight = 0f;
            this._fireSound = GetPath("SFX/pewpew");
        }

        public override void Update()
        {
            base.Update();
            if (this._aiming && (double)this._aimWait <= 0.0 && (double)this._fireAngle < 90)
                this._fireAngle += 3f;
            if ((double)this._aimWait > 0.0)
                this._aimWait -= 0.9f;
            if ((double)this._cooldown > 0.0)
                this._cooldown -= 0.1f;
            else
                this._cooldown = 0.0f;
            if (this.owner != null)
            {
                this._aimAngle = -Maths.DegToRad(this._fireAngle);
                if ((int)this.offDir < 0)
                    this._aimAngle = -this._aimAngle;
            }
            else
            {
                this._aimWait = 0.0f;
                this._aiming = false;
                this._aimAngle = 0.0f;
                this._fireAngle = 0.0f;
            }
            if (!this._raised)
                return;
            this._aimAngle = 0.0f;
        }

        public override void OnPressAction()
        {
            if ((double)this._cooldown != 0.0)
                return;
            if (this.ammo > 0)
            {
                this._aiming = true;
                this._aimWait = 1f;
            }
            else
                SFX.Play("click", 1f, 0.0f, 0.0f, false);
        }

        public override void OnReleaseAction()
        {
            if ((double)this._cooldown != 0.0 || this.ammo <= 0)
                return;
            this._aiming = false;
            this.Fire();
            this._cooldown = 1f;
            this.angle = 0.0f;
            this._fireAngle = 0.0f;
        }
    }
}
