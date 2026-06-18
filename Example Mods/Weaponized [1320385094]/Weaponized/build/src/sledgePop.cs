using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Melee")]
    [BaggedProperty("canSpawn", false)]
    public class SledgePop : Gun
    {
        public StateBinding _swingBinding = new StateBinding("_swing", -1, false, false);
        private Vec2 _offset = new Vec2();
        private SpriteMap _sprite;
        private SpriteMap _sledgeSwing;
        private float _swing;
        private float _swingLast;
        private float _swingVelocity;
        private float _swingForce;
        private bool _pressed;
        private float _lastSpeed;
        private int _lastDir;
        private float _fullSwing;
        private float _sparkWait;
        private bool _swung;
        private bool _drawOnce;
        private bool _held;
        private PhysicsObject _lastOwner;
        private float _hPull;

        public SledgePop(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = (AmmoType)new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("sledgePop"), 32, 32, false);
            this._sledgeSwing = new SpriteMap(GetPath("swingPop"), 32, 32, false);
            this._sledgeSwing.AddAnimation("swing", 0.8f, 0 != 0, 0, 1, 2, 3, 4, 5);
            this._sledgeSwing.currentAnimation = "swing";
            this._sledgeSwing.speed = 0.0f;
            this._sledgeSwing.center = new Vec2(16f, 16f);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(16f, 14f);
            this.collisionOffset = new Vec2(-2f, 0.0f);
            this.collisionSize = new Vec2(4f, 18f);
            this._barrelOffsetTL = new Vec2(16f, 28f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 3f;
            this.weight = 9f;
            this._dontCrush = true;
            this.collideSounds.Add("rockHitGround2");
            this._editorName = "Sledge Lollypop";
            this.editorTooltip = "Swing with this sweet hammer and fly awaaaaay!";
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!(with is IPlatform))
                return;
            for (int index = 0; index < 4; ++index)
                Level.Add((Thing)Spark.New(this.barrelPosition.x + Rando.Float(-6f, 6f), this.barrelPosition.y + Rando.Float(-3f, 3f), -MaterialThing.ImpactVector(from), 0.02f));
        }

        public override void CheckIfHoldObstructed()
        {
            Duck owner = this.owner as Duck;
            if (owner == null)
                return;
            owner.holdObstructed = false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void ReturnToWorld()
        {
            this.collisionOffset = new Vec2(-2f, 0.0f);
            this.collisionSize = new Vec2(4f, 18f);
            this._sprite.frame = 0;
            this._swing = 0.0f;
            this._swingForce = 0.0f;
            this._pressed = false;
            this._swung = false;
            this._fullSwing = 0.0f;
            this._swingVelocity = 0.0f;
        }

        public override void Update()
        {
            if (this._lastOwner != null && this.owner == null)
            {
                this._lastOwner.frictionMod = 0.0f;
                this._lastOwner = (PhysicsObject)null;
            }
            this.collisionOffset = new Vec2(-2f, 0.0f);
            this.collisionSize = new Vec2(4f, 18f);
            if ((double)this._swing > 0.0)
            {
                this.collisionOffset = new Vec2(-9999f, 0.0f);
                this.collisionSize = new Vec2(4f, 18f);
            }
            this._swingVelocity = Maths.LerpTowards(this._swingVelocity, this._swingForce, 0.2f);
            Duck owner = this.owner as Duck;
            if (this.isServerForObject)
            {
                this._swing += this._swingVelocity;
                float num1 = this._swing - this._swingLast;
                this._swingLast = this._swing;
                if ((double)this._swing > 1.0)
                    this._swing = 1f;
                if ((double)this._swing < 0.0)
                    this._swing = 0.0f;
                this._sprite.flipH = false;
                this._sprite.flipV = false;
                if ((double)this._sparkWait > 0.0)
                    this._sparkWait -= 0.1f;
                else
                    this._sparkWait = 0.0f;
                if (owner != null)
                {
                    if ((double)this._sparkWait == 0.0 && (double)this._swing == 0.0)
                    {
                        if (owner.grounded && (int)owner.offDir > 0 && (double)owner.hSpeed > 1.0)
                        {
                            this._sparkWait = 0.25f;
                            Level.Add((Thing)Spark.New(this.x - 22f, this.y + 6f, new Vec2(0.0f, 0.5f), 0.02f));
                        }
                        else if (owner.grounded && (int)owner.offDir < 0 && (double)owner.hSpeed < -1.0)
                        {
                            this._sparkWait = 0.25f;
                            Level.Add((Thing)Spark.New(this.x + 22f, this.y + 6f, new Vec2(0.0f, 0.5f), 0.02f));
                        }
                    }
                    float hSpeed = owner.hSpeed;
                    this._hPull = Maths.LerpTowards(this._hPull, owner.hSpeed, 0.15f);
                    if ((double)Math.Abs(owner.hSpeed) < 0.100000001490116)
                        this._hPull = 0.0f;
                    float num2 = Math.Abs(this._hPull) / 2.5f;
                    if ((double)num2 > 1.0)
                        num2 = 1f;
                    this.weight = (float)(8.0 - (double)num2 * 3.0);
                    if ((double)this.weight <= 5.0)
                        this.weight = 5.1f;
                    float num3 = Math.Abs(owner.hSpeed - this._hPull);
                    owner.frictionMod = 0.0f;
                    if ((double)owner.hSpeed > 0.0 && (double)this._hPull > (double)owner.hSpeed)
                        owner.frictionMod = (float)(-(double)num3 * 1.79999995231628);
                    if ((double)owner.hSpeed < 0.0 && (double)this._hPull < (double)owner.hSpeed)
                        owner.frictionMod = (float)(-(double)num3 * 1.79999995231628);
                    this._lastDir = (int)owner.offDir;
                    this._lastSpeed = hSpeed;
                    if ((double)this._swing != 0.0 && (double)num1 > 0.0)
                    {
                        owner.hSpeed += (float)((double)owner.offDir * ((double)num1 * 3.0) * (double)this.weightMultiplier);
                        owner.vSpeed -= (float)((double)num1 * 3.0 * (double)this.weightMultiplier);
                    }
                }
            }
            if ((double)this._swing < 0.5)
            {
                float num = this._swing * 2f;
                this._sprite.imageIndex = (int)((double)num * 10.0);
                this._sprite.angle = (float)(1.20000004768372 - (double)num * 1.5);
                this._sprite.yscale = (float)(1.0 - (double)num * 0.100000001490116);
            }
            else if ((double)this._swing >= 0.5)
            {
                float num = (float)(((double)this._swing - 0.5) * 2.0);
                this._sprite.imageIndex = 10 - (int)((double)num * 10.0);
                this._sprite.angle = (float)(-0.300000011920929 - (double)num * 1.5);
                this._sprite.yscale = (float)(1.0 - (1.0 - (double)num) * 0.100000001490116);
                this._fullSwing += 0.16f;
                if (!this._swung)
                {
                    this._swung = true;
                    if (this.duck != null && this.isServerForObject)
                        Level.Add((Thing)new popWave(this.x + (float)this.offDir * 4f + this.owner.hSpeed, this.y + 8f, (int)this.offDir, 0.15f, 4f + Math.Abs(this.owner.hSpeed), this.owner.vSpeed, this.duck));
                }
            }
            if ((double)this._swing == 1.0)
                this._pressed = false;
            if ((double)this._swing == 1.0 && !this._pressed && (double)this._fullSwing > 1.0)
            {
                this._swingForce = -0.08f;
                this._fullSwing = 0.0f;
            }
            if (this._sledgeSwing.finished)
                this._sledgeSwing.speed = 0.0f;
            this._lastOwner = this.owner as PhysicsObject;
            if (this.duck != null)
            {
                if (this.duck.action && !this._held && (double)this._swing == 0.0)
                {
                    this._fullSwing = 0.0f;
                    owner._disarmDisable = 30;
                    owner.crippleTimer = 1f;
                    this._sledgeSwing.speed = 1f;
                    this._sledgeSwing.frame = 0;
                    this._swingForce = 1f;
                    this._pressed = true;
                    this._swung = false;
                    this._held = true;
                }
                if (!this.duck.action)
                {
                    this._pressed = false;
                    this._held = false;
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            if (this.owner != null && this._drawOnce)
            {
                this._offset = new Vec2((float)((double)this.offDir * -6.0 + (double)this._swing * 5.0 * (double)this.offDir), (float)((double)this._swing * 5.0 - 3.0));
                this.graphic.position = this.position + this._offset;
                this.graphic.depth = this.depth;
                Duck owner = this.owner as Duck;
                this.handOffset = new Vec2(this._swing * 3f, (float)(0.0 - (double)this._swing * 4.0));
                this.handAngle = (float)(1.39999997615814 + ((double)this._sprite.angle * 0.5 - 1.0));
                if (owner != null && (int)owner.offDir < 0)
                {
                    this._sprite.angle = -this._sprite.angle;
                    this.handAngle = -this.handAngle;
                }
                this.graphic.Draw();
                if ((double)this._sledgeSwing.speed <= 0.0)
                    return;
                if (owner != null)
                    this._sledgeSwing.flipH = (int)owner.offDir <= 0;
                this._sledgeSwing.position = this.position;
                this._sledgeSwing.depth = this.depth + 1;
                this._sledgeSwing.Draw();
            }
            else
            {
                base.Draw();
                this._drawOnce = true;
            }
        }

        public override void OnPressAction()
        {
        }

        public override void OnReleaseAction()
        {
        }

        public override void Fire()
        {
        }
    }
}