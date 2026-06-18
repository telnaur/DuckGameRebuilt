using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Explosives")]
    public class pirateCannon : TampingWeapon
    {
        public float _fireAngle;
        public float _aimAngle;
        public float _aimWait;
        public bool _aiming;
        public float _cooldown;
        public SpriteMap _sprite;

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

        public pirateCannon(float xval, float yval)
          : base(xval, yval)
        {
            this.wideBarrel = true;
            this.ammo = 99;
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("pirateCannon"), 19, 14, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(10f, 7f);
            this.collisionOffset = new Vec2(-10f, -5f);
            this.collisionSize = new Vec2(19f, 10f);
            this._barrelOffsetTL = new Vec2(19f, 7f);
            this._kickForce = 5f;
            this._fireRumble = RumbleIntensity.Heavy;
            this._holdOffset = new Vec2(4f, 0.0f);
            this._ammoType = (AmmoType)new ATCannon();
            this._fireSound = "missile";
            this._bulletColor = Color.White;
            this.weight = 8f;
            this._dontCrush = true;
            this._sprite.AddAnimation("idle", 1f, true, 2);
            this._sprite.AddAnimation("fire", 0.07f, false, 2, 1, 0, 0);
            this._sprite.SetAnimation("idle");
            this._editorName = "Pirate Cannon";
            this.editorTooltip = "Yarr, aim this heavy cannon at enemy ships and blow up their footing.";
        }

        public override void Update()
        {
            base.Update();
            if (this._aiming && (double)this._aimWait <= 0.0 && (double)this._fireAngle < 81.0)
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
                if (this.offDir < (sbyte)0)
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
            if (this._tamped)
            {
                if ((double)this._cooldown != 0.0)
                    return;
                if (this.ammo > 0)
                {
                    this._sprite.SetAnimation("fire");
                    this._aiming = true;
                    this._aimWait = 1f;
                }
                else
                    SFX.Play("lightMatch", 1f, 0.0f, 0.0f, false);
                this._tampInc = 0.0f;
                this._tampTime = this.infinite.value ? 0.5f : 0.0f;
            }
            else
            {
                if (this._raised)
                    return;
                Duck owner = this.owner as Duck;
                if (owner == null || !owner.grounded)
                    return;
                owner.immobilized = true;
                owner.sliding = false;
                this._rotating = true;
            }
        }

        public override void OnReleaseAction()
        {
            this._sprite.SetAnimation("idle");
            if ((double)this._cooldown != 0.0 || this.ammo <= 0)
                return;
            this._aiming = false;
            if (this._tamped)
            {
                this.Fire();
                this._tamped = false;
            }
            this._cooldown = 1f;
            this.angle = 0.0f;
            this._fireAngle = 0.0f;
        }
    }
}
