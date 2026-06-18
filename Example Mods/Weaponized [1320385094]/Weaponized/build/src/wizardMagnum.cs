using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{


    [EditorGroup("Zyrafa|Guns|Pistols")]
    public class wizardMagnum : Gun
    {
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

        public wizardMagnum(float xval, float yval)
          : base(xval, yval)
        {

            this.ammo = 6;
            this._ammoType = (AmmoType)new ATwizard();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("wizardMagnum"), 0.0f, 0.0f);
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -6f);
            this.collisionSize = new Vec2(16f, 10f);
            this._barrelOffsetTL = new Vec2(25f, 7f);
            this._fireSound = "magnum";
            this._kickForce = 3f;
            this._holdOffset = new Vec2(1f, 2f);
            this.handOffset = new Vec2(0.0f, 1f);
            this._ammoType.barrelAngleDegrees = -45f;
            this._numBulletsPerFire = 2;
            this._editorName = "Wizard Magnum";
            this.editorTooltip = "Shooting at two ducks at the same time, magic!";
        }

        public override void Update()
        {
            base.Update();
            this._angleOffset = this.owner == null ? 0.0f : (this.offDir >= (sbyte)0 ? -Maths.DegToRad(this.rise * 65f) : -Maths.DegToRad((float)(-(double)this.rise * 65.0)));
            if ((double)this.rise > 0.0)
                this.rise -= 0.013f;
            else
                this.rise = 0.0f;
            if (!this._raised)
                return;
            this._angleOffset = 0.0f;
        }

        public override void Fire()
        {
            if (!this.loaded)
                return;
            if (this.ammo > 0 && (double)this._wait == 0.0)
            {
                this.firedBullets.Clear();
                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None, RumbleType.Gameplay));
                this.ApplyKick();
                this._ammoType.barrelAngleDegrees = -45f;
                this._barrelOffsetTL = new Vec2(25f, 7f);
                for (int index = 0; index < this._numBulletsPerFire; ++index)
                {
                    float accuracy = this._ammoType.accuracy;
                    this._ammoType.accuracy *= 1f - this._accuracyLost;
                    this._ammoType.bulletColor = this._bulletColor;
                    float angleDegrees = this.angleDegrees;
                    float angle = this.offDir >= (sbyte)0 ? angleDegrees + this._ammoType.barrelAngleDegrees : angleDegrees + 180f - this._ammoType.barrelAngleDegrees;
                    if (!this.receivingPress)
                    {
                        Bullet bullet = this._ammoType.FireBullet(this.Offset(this.barrelOffset), this.owner, angle, (Thing)this);
                        if (Network.isActive && this.isServerForObject)
                        {
                            this.firedBullets.Add(bullet);
                            if (this.duck != null && this.duck.profile.connection != null)
                                bullet.connection = this.duck.profile.connection;
                        }
                    }
                    ++this.bulletFireIndex;
                    this._ammoType.accuracy = accuracy;
                    this._barrelHeat += 0.3f;
                    this._ammoType.barrelAngleDegrees = 45f;
                    this._barrelOffsetTL = new Vec2(25f, 13f);
                }
                this._smokeWait = 3f;
                this.loaded = false;
                this._flareAlpha = 1.5f;
                if (!this._manualLoad)
                    this.Reload(true);
                this.firing = true;
                this._wait = this._fireWait;
                this.PlayFireSound();
                if (this.owner == null)
                {
                    Vec2 vec2 = this.barrelVector * Rando.Float(1f, 3f);
                    vec2.y += Rando.Float(2f);
                    this.hSpeed -= vec2.x;
                    this.vSpeed -= vec2.y;
                }
                this._accuracyLost += this.loseAccuracy;
                if ((double)this._accuracyLost <= (double)this.maxAccuracyLost)
                    return;
                this._accuracyLost = this.maxAccuracyLost;
            }
            else
            {
                if (this.ammo > 0 || (double)this._wait != 0.0)
                    return;
                this.firedBullets.Clear();
                this.DoAmmoClick();
                this._wait = this._fireWait;
            }
        }
        public override void OnPressAction()
        {
            base.OnPressAction();
            if (this.ammo <= 0 || (double)this.rise >= 1.0)
                return;
            this.rise += 0.4f;
            /*this._ammoType.barrelAngleDegrees = 45f;
            this._fireWait = 0f;
            this._wait = 0f;
            this._barrelOffsetTL = new Vec2(25f, 13f);
            base.OnPressAction();
            this._ammoType.barrelAngleDegrees = -45f;
            this._fireWait = 1f;
            this._wait = 1f;
            this._barrelOffsetTL = new Vec2(25f, 7f);*/
        }//sprobuj w 2 osobnych klatkach odpalic base onpressaction i przetestuj odrzut magnuma
    }
}
