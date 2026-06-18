using System;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Western")]
    public class Gunblade : Gun
    {
        public StateBinding _swingBinding = new StateBinding("_swing", -1, false);

        public StateBinding _holdBinding = new StateBinding("_hold", -1, false);

        public StateBinding _jabStanceBinding = new StateBinding("_jabStance", -1, false);

        public StateBinding _crouchStanceBinding = new StateBinding("_crouchStance", -1, false);

        public StateBinding _slamStanceBinding = new StateBinding("_slamStance", -1, false);

        public StateBinding _pullBackBinding = new StateBinding("_pullBack", -1, false);

        public StateBinding _swingingBinding = new StateBinding("_swinging", -1, false);

        public StateBinding _throwSpinBinding = new StateBinding("_throwSpin", -1, false);

        public StateBinding _volatileBinding = new StateBinding("_volatile", -1, false);

        public float _swing;

        public float _hold;

        private bool _drawing;

        public bool _pullBack;

        public bool _jabStance;

        public bool _crouchStance;

        public bool _slamStance;

        public bool _swinging;

        public float _addOffsetX;

        public float _addOffsetY;

        public bool _swingPress;

        public bool _shing;

        public static bool _playedShing;

        public bool _atRest = true;

        public bool _swung;

        public bool _wasLifted;

        public float _throwSpin;

        public int _framesExisting;

        public int _hitWait;

        private SpriteMap _swordSwing;

        private int _unslam;

        private byte blocked;

        public bool _volatile;

        private System.Collections.Generic.List<float> _lastAngles = new System.Collections.Generic.List<float>();

        private System.Collections.Generic.List<Vec2> _lastPositions = new System.Collections.Generic.List<Vec2>();

        public override float angle
        {
            get
            {
                if (this._drawing)
                {
                    return this._angle;
                }
                return base.angle + (this._swing + this._hold) * (float)this.offDir;
            }
            set
            {
                this._angle = value;
            }
        }

        public bool jabStance
        {
            get
            {
                return this._jabStance;
            }
        }

        public bool crouchStance
        {
            get
            {
                return this._crouchStance;
            }
        }

        public Vec2 barrelStartPos
        {
            get
            {
                if (this.owner == null)
                {
                    return this.position - (this.Offset(base.barrelOffset) - this.position).normalized * 6f;
                }
                if (this._slamStance)
                {
                    return this.position + (this.Offset(base.barrelOffset) - this.position).normalized * 12f;
                }
                return this.position + (this.Offset(base.barrelOffset) - this.position).normalized * 2f;
            }
        }

        public Gunblade(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Gunblade";
            this.ammo = 4;
            this._ammoType = new ATSniper();
            this._ammoType.range = 170f;
            this._type = "gun";
            base.graphic = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("GunBlade"), 8, 23, false);
            this.center = new Vec2(3f, 18f);
            this.collisionOffset = new Vec2(-3f, -18f);
            this.collisionSize = new Vec2(8f, 23f);
            this._barrelOffsetTL = new Vec2(2f, 0f);
            this._fireSound = GetPath("sounds/revolver");
            this._fullAuto = true;
            this._fireWait = 1f;
            this._holdOffset = new Vec2(-4f, 4f);
            this.weight = 0.9f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._swordSwing = new SpriteMap("swordSwipe", 32, 32, false);
            this._swordSwing.AddAnimation("swing", 0.6f, false, new int[]
            {
                0,
                1,
                1,
                2
            });
           
            this._swordSwing.currentAnimation = "swing";
            this._swordSwing.speed = 0f;
            this._swordSwing.center = new Vec2(9f, 25f);
            this._bouncy = 0.5f;
            this._impactThreshold = 0.3f;
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            return false;
        }

        public override void UpdateFirePosition(SmallFire f)
        {
            f.graphic.color = Color.White;
        }

        public override void AddFire()
        {
            return;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void CheckIfHoldObstructed()
        {
            Duck duck = this.owner as Duck;
            if (duck != null)
            {
                duck.holdObstructed = false;
            }
        }

        public override void Thrown()
        {
        }

        public void Shing()
        {
            if (!this._shing)
            {
                this._pullBack = false;
                this._swinging = false;
                this._shing = true;
                this._swingPress = false;
                if (!Gunblade._playedShing)
                {
                    Gunblade._playedShing = true;
                    SFX.Play("swordClash", Rando.Float(0.6f, 0.7f), Rando.Float(-0.1f, 0.1f), Rando.Float(-0.1f, 0.1f), false);
                }
                Vec2 normalized = (this.position - base.barrelPosition).normalized;
                Vec2 value = base.barrelPosition;
                for (int i = 0; i < 6; i++)
                {
                    Level.Add(Spark.New(value.x, value.y, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)), 0.02f));
                    value += normalized * 4f;
                }
                this._swung = false;
                this._swordSwing.speed = 0f;
            }
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (base.duck != null)
            {
                if (this.blocked == 0)
                {
                    base.duck.AddCoolness(1);
                }
                else
                {
                    this.blocked += 1;
                    if (this.blocked > 4)
                    {
                        this.blocked = 1;
                        base.duck.AddCoolness(1);
                    }
                }
                float angleAdjust = 0f;
                float xHit = position.x - hitPos.x;
                float yHit = position.y - hitPos.y;
                bool isXHit = Math.Abs(xHit) > Math.Abs(yHit);
                float xAdjust = xHit > 0 ? -1f : 1f;
                float yAdjust = yHit > 0 ? -1f : 1f;
                bool xBlocked = Level.CheckPoint<Block>(hitPos.x + xAdjust, hitPos.y) != null;
                bool yBlocked = Level.CheckPoint<Block>(hitPos.x, hitPos.y + yAdjust) != null;
                if (isXHit && xBlocked)
                    isXHit = false;
                if (!isXHit && yBlocked)
                    isXHit = true;
                if (xBlocked && yBlocked)
                    return true;
                if (isXHit)
                    angleAdjust = 180f;
                Bullet reboundBullet = bullet.ammo.GetBullet(hitPos.x, hitPos.y, this, bullet.angle + angleAdjust, bullet.firedFrom, bullet.range - ((bullet.bulletDistance > 0f) ? bullet.bulletDistance : bullet.range / 2f));
                Level.Add(reboundBullet);
                SFX.Play("ting", 1f, 0f, 0f, false);
                return base.Hit(bullet, hitPos);
            }
            return false;
        }


        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._wasLifted && this.owner == null && with is Block)
            {
                this.Shing();
                this._framesSinceThrown = 15;
            }
        }

        public override void ReturnToWorld()
        {
            this._throwSpin = 90f;
            this.collisionOffset = new Vec2(-2f, -16f);
            this.collisionSize = new Vec2(4f, 18f);
            if (this._wasLifted)
            {
                this.collisionOffset = new Vec2(-4f, -2f);
                this.collisionSize = new Vec2(8f, 4f);
            }
        }

        public override void Update()
        {
            base.Update();
            try
            {
                this.burnt = 0.9f;
                this.burnSpeed = 0f;
                foreach (SmallFire sf in Level.CheckRectAll<SmallFire>(base.topLeft, base.bottomRight))
                {
                    sf.graphic.color = Color.White;
                    sf.SuckLife(15f);
                }
            }
            catch { }
            if (base.equippedDuck != null)
            {
                float speedplus = 0.2f;
                if (!base.equippedDuck.sliding && !base.equippedDuck.immobilized && !base.equippedDuck.moveLock)
                {
                    if (base.equippedDuck.grounded == false)
                    {
                        speedplus = 0.1f;
                    }
                    if (base.equippedDuck.inputProfile.Down("RIGHT") && base.equippedDuck.hSpeed < 6f)
                    {
                        base.equippedDuck.hSpeed = MathHelper.Lerp(base.equippedDuck.hSpeed, 6f, speedplus);
                    }
                    if (base.equippedDuck.inputProfile.Down("LEFT") && base.equippedDuck.hSpeed > -6f)
                    {
                        base.equippedDuck.hSpeed = MathHelper.Lerp(base.equippedDuck.hSpeed, -6f, speedplus);
                    }
                }
            }
            if (this._swordSwing.finished)
            {
                this._swordSwing.speed = 0f;
            }
            if (this._hitWait > 0)
            {
                this._hitWait--;
            }
            this._framesExisting++;
            if (this._framesExisting > 100)
            {
                this._framesExisting = 100;
            }
            if (System.Math.Abs(this.hSpeed) + System.Math.Abs(this.vSpeed) > 4f)
            {
                this._wasLifted = true;
            }
            if (this.owner != null)
            {
                this._hold = -0.4f;
                this._wasLifted = true;
                this.center = new Vec2(4f, 21f);
                this._framesSinceThrown = 0;
            }
            else
            {
                if (this._framesSinceThrown == 1)
                {
                    this._throwSpin = Maths.RadToDeg(this.angle) - 90f;
                    this._hold = 0f;
                    this._swing = 0f;
                }
                if (this._wasLifted)
                {
                    base.angleDegrees = 90f + this._throwSpin;
                    this.center = new Vec2(4f, 11f);
                }
                this._volatile = false;
                bool flag = false;
                bool flag2 = false;
                if (System.Math.Abs(this.hSpeed) + System.Math.Abs(this.vSpeed) > 2f || !base.grounded)
                {
                    if (!base.grounded)
                    {
                        Block block = Level.CheckRect<Block>(this.position + new Vec2(-6f, -6f), this.position + new Vec2(6f, -2f), null);
                        if (block != null)
                        {
                            flag2 = true;
                            if (this.vSpeed > 4f)
                            {
                                this._volatile = true;
                            }
                        }
                    }
                    if (!flag2 && !this._grounded && Level.CheckPoint<IPlatform>(this.position + new Vec2(0f, 8f), null, null) == null)
                    {
                        if (this.hSpeed > 0f)
                        {
                            this._throwSpin += (System.Math.Abs(this.hSpeed) + System.Math.Abs(this.vSpeed)) * 2f + 4f;
                        }
                        else
                        {
                            this._throwSpin -= (System.Math.Abs(this.hSpeed) + System.Math.Abs(this.vSpeed)) * 2f + 4f;
                        }
                        flag = true;
                    }
                }
                if (this._framesExisting > 15 && System.Math.Abs(this.hSpeed) + System.Math.Abs(this.vSpeed) > 3f)
                {
                    this._volatile = true;
                }
                if (!flag || flag2)
                {
                    this._throwSpin %= 360f;
                    if (flag2)
                    {
                        if (System.Math.Abs(this._throwSpin - 90f) < System.Math.Abs(this._throwSpin + 90f))
                        {
                            this._throwSpin = Lerp.Float(this._throwSpin, 90f, 16f);
                        }
                        else
                        {
                            this._throwSpin = Lerp.Float(-90f, 0f, 16f);
                        }
                    }
                    else if (this._throwSpin > 90f && this._throwSpin < 270f)
                    {
                        this._throwSpin = Lerp.Float(this._throwSpin, 180f, 14f);
                    }
                    else
                    {
                        if (this._throwSpin > 180f)
                        {
                            this._throwSpin -= 360f;
                        }
                        else if (this._throwSpin < -180f)
                        {
                            this._throwSpin += 360f;
                        }
                        this._throwSpin = Lerp.Float(this._throwSpin, 0f, 14f);
                    }
                }
                if (this._volatile && this._hitWait == 0)
                {
                    (this.Offset(base.barrelOffset) - this.position).Normalize();
                    this.Offset(base.barrelOffset);
                    bool flag3 = false;
                    using (System.Collections.Generic.IEnumerator<Thing> enumerator = Level.current.things[typeof(Gunblade)].GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Gunblade sword = (Gunblade)enumerator.Current;
                            if (sword != this && sword.owner != null && sword._crouchStance && !sword._jabStance && !sword._jabStance && ((this.hSpeed > 0f && sword.x > base.x - 4f) || (this.hSpeed < 0f && sword.x < base.x + 4f)) && Collision.LineIntersect(this.barrelStartPos, base.barrelPosition, sword.barrelStartPos, sword.barrelPosition))
                            {
                                this.Shing();
                                sword.Shing();
                                sword.owner.hSpeed += (float)this.offDir * 1f;
                                sword.owner.vSpeed -= 1f;
                                flag3 = true;
                                this._hitWait = 1;
                                this.hSpeed = -this.hSpeed * 0.6f;
                            }
                        }
                    }
                    int num = 12;
                    if (!flag3)
                    {
                        using (System.Collections.Generic.IEnumerator<Thing> enumerator2 = Level.current.things[typeof(Chainsaw)].GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                Chainsaw chainsaw = (Chainsaw)enumerator2.Current;
                                if (chainsaw.owner != null && chainsaw.throttle && Collision.LineIntersect(this.barrelStartPos, base.barrelPosition, chainsaw.barrelStartPos, chainsaw.barrelPosition))
                                {
                                    this.Shing();
                                    chainsaw.Shing(this);
                                    chainsaw.owner.hSpeed += (float)this.offDir * 1f;
                                    chainsaw.owner.vSpeed -= 1f;
                                    flag3 = true;
                                    this.hSpeed = -this.hSpeed * 0.6f;
                                    this._hitWait = 1;
                                    if (Recorder.currentRecording != null)
                                    {
                                        Recorder.currentRecording.LogBonus();
                                    }
                                }
                            }
                        }
                        if (!flag3)
                        {
                            Helmet helmet = Level.CheckLine<Helmet>(this.barrelStartPos, base.barrelPosition, null);
                            if (helmet != null && helmet.equippedDuck != null && (helmet.owner != base.prevOwner || (int)this._framesSinceThrown > num))
                            {
                                this.hSpeed = -this.hSpeed * 0.6f;
                                this.Shing();
                                flag3 = true;
                                this._hitWait = 1;
                            }
                            else
                            {
                                ChestPlate chestPlate = Level.CheckLine<ChestPlate>(this.barrelStartPos, base.barrelPosition, null);
                                if (chestPlate != null && chestPlate.equippedDuck != null && (chestPlate.owner != base.prevOwner || (int)this._framesSinceThrown > num))
                                {
                                    this.hSpeed = -this.hSpeed * 0.6f;
                                    this.Shing();
                                    flag3 = true;
                                    this._hitWait = 1;
                                }
                            }
                        }
                    }
                    if (!flag3 && base.isServerForObject)
                    {
                        System.Collections.Generic.IEnumerable<IAmADuck> enumerable = Level.CheckLineAll<IAmADuck>(this.barrelStartPos, base.barrelPosition);
                        foreach (IAmADuck current in enumerable)
                        {
                            if (current != base.duck)
                            {
                                MaterialThing materialThing = current as MaterialThing;
                                if (materialThing != null && (materialThing != base.prevOwner || (int)this._framesSinceThrown > num))
                                {
                                    materialThing.Destroy(new DTImpale(this));
                                    if (Recorder.currentRecording != null)
                                    {
                                        Recorder.currentRecording.LogBonus();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (this.owner == null)
            {
                this._swinging = false;
                this._jabStance = false;
                this._crouchStance = false;
                this._pullBack = false;
                this._swung = false;
                this._shing = false;
                this._swing = 0f;
                this._swingPress = false;
                this._slamStance = false;
                this._unslam = 0;
            }
            if (this._unslam > 1)
            {
                this._unslam--;
                this._slamStance = true;
            }
            else if (this._unslam == 1)
            {
                this._unslam = 0;
                this._slamStance = false;
            }
            if (this._pullBack)
            {
                if (base.duck != null)
                {
                    if (this._jabStance)
                    {
                        this._pullBack = false;
                        this._swinging = true;
                    }
                    else
                    {
                        this._swinging = true;
                        this._pullBack = false;
                    }
                }
            }
            else if (this._swinging)
            {
                if (this._jabStance)
                {
                    this._addOffsetX = MathHelper.Lerp(this._addOffsetX, 3f, 0.4f);
                    if (this._addOffsetX > 2f && !this.action)
                    {
                        this._swinging = false;
                    }
                }
                else if (base.raised)
                {
                    this._swing = MathHelper.Lerp(this._swing, -2.8f, 0.2f);
                    if (this._swing < -2.4f && !this.action)
                    {
                        this._swinging = false;
                        this._swing = 1.8f;
                    }
                }
                else
                {
                    this._swing = MathHelper.Lerp(this._swing, 2.1f, 0.4f);
                    if (this._swing > 1.8f && !this.action)
                    {
                        this._swinging = false;
                        this._swing = 1.8f;
                    }
                }
            }
            else
            {
                if (!this._swinging && (!this._swingPress || this._shing || (this._jabStance && this._addOffsetX < 1f) || (!this._jabStance && this._swing < 1.6f)))
                {
                    if (this._jabStance)
                    {
                        this._swing = MathHelper.Lerp(this._swing, 1.75f, 0.4f);
                        if (this._swing > 1.55f)
                        {
                            this._swing = 1.55f;
                            this._shing = false;
                            this._swung = false;
                        }
                        this._addOffsetX = MathHelper.Lerp(this._addOffsetX, -12f, 0.45f);
                        if (this._addOffsetX < -12f)
                        {
                            this._addOffsetX = -12f;
                        }
                        this._addOffsetY = MathHelper.Lerp(this._addOffsetY, -4f, 0.35f);
                        if (this._addOffsetX < -3f)
                        {
                            this._addOffsetY = -3f;
                        }
                    }
                    else if (this._slamStance)
                    {
                        this._swing = MathHelper.Lerp(this._swing, 3.14f, 0.8f);
                        if (this._swing > 3.1f && this._unslam == 0)
                        {
                            this._swing = 3.14f;
                            this._shing = false;
                            this._swung = true;
                        }
                        this._addOffsetX = MathHelper.Lerp(this._addOffsetX, -5f, 0.45f);
                        if (this._addOffsetX < -4.6f)
                        {
                            this._addOffsetX = -5f;
                        }
                        this._addOffsetY = MathHelper.Lerp(this._addOffsetY, -6f, 0.35f);
                        if (this._addOffsetX < -5.5f)
                        {
                            this._addOffsetY = -6f;
                        }
                    }
                    else
                    {
                        this._swing = MathHelper.Lerp(this._swing, -0.22f, 0.36f);
                        this._addOffsetX = MathHelper.Lerp(this._addOffsetX, 1f, 0.2f);
                        if (this._addOffsetX > 0f)
                        {
                            this._addOffsetX = 0f;
                        }
                        this._addOffsetY = MathHelper.Lerp(this._addOffsetY, 1f, 0.2f);
                        if (this._addOffsetY > 0f)
                        {
                            this._addOffsetY = 0f;
                        }
                    }
                }
                if ((this._swing < 0f || this._jabStance) && this._swing < 0f)
                {
                    this._swing = 0f;
                    this._shing = false;
                    this._swung = false;
                }
            }
            if (base.duck != null)
            {
                this.collisionOffset = new Vec2(-4f, 0f);
                this.collisionSize = new Vec2(4f, 4f);
                if (this._crouchStance && !this._jabStance)
                {
                    this.collisionOffset = new Vec2(-2f, -19f);
                    this.collisionSize = new Vec2(4f, 16f);
                    this.thickness = 5f;
                }
                this._swingPress = false;
                if (!this._pullBack && !this._swinging)
                {
                    this._crouchStance = false;
                    this._jabStance = false;
                    if (base.duck.crouch)
                    {
                        if (!this._pullBack && !this._swinging && base.duck.inputProfile.Down((this.offDir > 0) ? "LEFT" : "RIGHT"))
                        {
                            this._jabStance = true;
                        }
                        this._crouchStance = true;
                    }
                    if (!this._crouchStance || this._jabStance)
                    {
                        this._slamStance = false;
                    }
                }
                if (!this._crouchStance)
                {
                    this._hold = -0.4f;
                    this.handOffset = new Vec2(this._addOffsetX, this._addOffsetY);
                    this._holdOffset = new Vec2(-4f + this._addOffsetX, 4f + this._addOffsetY);
                }
                else
                {
                    this._hold = 0f;
                    this._holdOffset = new Vec2(0f + this._addOffsetX, 4f + this._addOffsetY);
                    this.handOffset = new Vec2(3f + this._addOffsetX, this._addOffsetY);
                }
            }
            else
            {
                this.collisionOffset = new Vec2(-2f, -16f);
                this.collisionSize = new Vec2(4f, 18f);
                if (this._wasLifted)
                {
                    this.collisionOffset = new Vec2(-4f, -2f);
                    this.collisionSize = new Vec2(8f, 4f);
                }
                this.thickness = 1f;
            }
            if ((this._swung || this._swinging) && !this._shing)
            {
                (this.Offset(base.barrelOffset) - this.position).Normalize();
                this.Offset(base.barrelOffset);
                System.Collections.Generic.IEnumerable<IAmADuck> enumerable2 = Level.CheckLineAll<IAmADuck>(this.barrelStartPos, base.barrelPosition);
                Block block2 = Level.CheckLine<Block>(this.barrelStartPos, base.barrelPosition, null);
                if (block2 != null && !this._slamStance)
                {
                    if (this.offDir < 0 && block2.x > base.x)
                    {
                        block2 = null;
                    }
                    else if (this.offDir > 0 && block2.x < base.x)
                    {
                        block2 = null;
                    }
                }
                bool flag4 = false;
                if (block2 != null)
                {
                    this.Shing();
                    if (this._slamStance)
                    {
                        this._swung = false;
                        this._unslam = 20;
                        this.owner.vSpeed = -5f;
                    }
                    if (block2 is Window)
                    {
                        block2.Destroy(new DTImpact(this));
                    }
                }
                else if (!this._jabStance && !this._slamStance)
                {
                    Thing ignore = null;
                    if (base.duck != null)
                    {
                        ignore = base.duck.GetEquipment(typeof(Helmet));
                    }
                    Vec2 vec = base.barrelPosition + base.barrelVector * 3f;
                    Vec2 p = new Vec2((this.position.x < vec.x) ? this.position.x : vec.x, (this.position.y < vec.y) ? this.position.y : vec.y);
                    Vec2 p2 = new Vec2((this.position.x > vec.x) ? this.position.x : vec.x, (this.position.y > vec.y) ? this.position.y : vec.y);
                    QuadLaserBullet quadLaserBullet = Level.CheckRect<QuadLaserBullet>(p, p2, null);
                    if (quadLaserBullet != null)
                    {
                        this.Shing();
                        base.Fondle(quadLaserBullet);
                        quadLaserBullet.safeFrames = 8;
                        quadLaserBullet.safeDuck = base.duck;
                        Vec2 travel = quadLaserBullet.travel;
                        float length = travel.length;
                        float num2 = 1f;
                        if (this.offDir > 0 && travel.x < 0f)
                        {
                            num2 = 1.5f;
                        }
                        else if (this.offDir < 0 && travel.x > 0f)
                        {
                            num2 = 1.5f;
                        }
                        if (this.offDir > 0)
                        {
                            travel = new Vec2(length * num2, 0f);
                        }
                        else
                        {
                            travel = new Vec2(-length * num2, 0f);
                        }
                        quadLaserBullet.travel = travel;
                    }
                    Bullet bullet = Level.CheckRect<Bullet>(p, p2, null);
                    if (bullet != null)
                    {
                        if(bullet.ammo.bulletThickness <= 2)
                        {
                            bullet.Removed();
                        }
                    }
                    else
                    {
                        Helmet helmet2 = Level.CheckLine<Helmet>(this.barrelStartPos, base.barrelPosition, ignore);
                        if (helmet2 != null && helmet2.equippedDuck != null)
                        {
                            this.Shing();
                            helmet2.owner.hSpeed += (float)this.offDir * 3f;
                            helmet2.owner.vSpeed -= 2f;
                            helmet2.duck.crippleTimer = 1f;
                            if(helmet2.isArmor && helmet2.thickness <= 10)
                            {

                            }
                            helmet2.Destroy(new DTImpale(this));
                            flag4 = true;
                            if (helmet2.owner is Duck)
                            {
                                ((Duck)helmet2.owner).Kill(new DTImpale(this));
                            }
                        }
                        else
                        {
                            if (base.duck != null)
                            {
                                ignore = base.duck.GetEquipment(typeof(ChestPlate));
                            }
                            ChestPlate chestPlate2 = Level.CheckLine<ChestPlate>(this.barrelStartPos, base.barrelPosition, ignore);
                            if (chestPlate2 != null && chestPlate2.equippedDuck != null)
                            {
                                this.Shing();
                                chestPlate2.owner.hSpeed += (float)this.offDir * 3f;
                                chestPlate2.owner.vSpeed -= 2f;
                                chestPlate2.duck.crippleTimer = 1f;
                                chestPlate2.Destroy(new DTImpale(this));
                                if (chestPlate2.owner is Duck)
                                {
                                    ((Duck)chestPlate2.owner).Kill(new DTImpale(this));
                                }
                                flag4 = true;
                            }
                        }
                    }
                }
                if (!flag4)
                {
                    using (System.Collections.Generic.IEnumerator<Thing> enumerator4 = Level.current.things[typeof(Gunblade)].GetEnumerator())
                    {
                        while (enumerator4.MoveNext())
                        {
                            Gunblade sword2 = (Gunblade)enumerator4.Current;
                            if (sword2 != this && sword2.duck != null && !this._jabStance && !sword2._jabStance && base.duck != null && Collision.LineIntersect(this.barrelStartPos, base.barrelPosition, sword2.barrelStartPos, sword2.barrelPosition))
                            {
                                this.Shing();
                                sword2.Shing();
                                sword2.owner.hSpeed += (float)this.offDir * 3f;
                                sword2.owner.vSpeed -= 3f;
                                base.duck.hSpeed += (float)(-(float)this.offDir) * 3f;
                                base.duck.vSpeed -= 3f;
                                sword2.duck.crippleTimer = 1f;
                                base.duck.crippleTimer = 1f;
                                flag4 = true;
                            }
                        }
                    }
                }
                if (flag4)
                {
                    return;
                }
                using (System.Collections.Generic.IEnumerator<IAmADuck> enumerator5 = enumerable2.GetEnumerator())
                {
                    while (enumerator5.MoveNext())
                    {
                        IAmADuck current2 = enumerator5.Current;
                        if (current2 != base.duck)
                        {
                            MaterialThing materialThing2 = current2 as MaterialThing;
                            if (materialThing2 != null)
                            {
                                materialThing2.Destroy(new DTImpale(this));
                            }
                        }
                    }
                    return;
                }
            }
            if (this._crouchStance && base.duck != null)
            {
                System.Collections.Generic.IEnumerable<IAmADuck> enumerable3 = Level.CheckLineAll<IAmADuck>(this.barrelStartPos, base.barrelPosition);
                foreach (IAmADuck current3 in enumerable3)
                {
                    if (current3 != base.duck)
                    {
                        MaterialThing materialThing3 = current3 as MaterialThing;
                        if (materialThing3 != null)
                        {
                            if (materialThing3.vSpeed > 0.5f && materialThing3.bottom < this.position.y - 8f && materialThing3.left < base.barrelPosition.x && materialThing3.right > base.barrelPosition.x)
                            {
                                materialThing3.Destroy(new DTImpale(this));
                            }
                            else if (!this._jabStance && !materialThing3.destroyed && ((this.offDir > 0 && materialThing3.x > base.duck.x) || (this.offDir < 0 && materialThing3.x < base.duck.x)))
                            {
                                if (materialThing3 is Duck)
                                {
                                    (materialThing3 as Duck).crippleTimer = 1f;
                                }
                                else if ((base.duck.x > materialThing3.x && materialThing3.hSpeed > 1.5f) || (base.duck.x < materialThing3.x && materialThing3.hSpeed < -1.5f))
                                {
                                    materialThing3.Destroy(new DTImpale(this));
                                }
                                base.Fondle(materialThing3);
                                materialThing3.hSpeed = (float)this.offDir * 3f;
                                materialThing3.vSpeed = -2f;
                            }
                        }
                    }
                }
            }
        }

        public override void Draw()
        {
            Gunblade._playedShing = false;
            if (this._swordSwing.speed > 0f)
            {
                if (base.duck != null)
                {
                    this._swordSwing.flipH = (base.duck.offDir <= 0);
                }
                this._swordSwing.alpha = 0.4f;
                this._swordSwing.position = this.position;
                this._swordSwing.depth = base.depth + 1;
                this._swordSwing.Draw();
            }
            base.alpha = 1f;
            Vec2 position = this.position;
            Depth depth = base.depth;
            base.graphic.color = Color.White;
            if ((this.owner == null && base.velocity.length > 1f) || this._swing != 0f)
            {
                float angle = this.angle;
                this._drawing = true;
                float angle2 = this._angle;
                this.angle = angle;
                for (int i = 0; i < 7; i++)
                {
                    base.Draw();
                    if (this._lastAngles.Count > i)
                    {
                        this._angle = this._lastAngles[i];
                    }
                    if (this._lastPositions.Count <= i)
                    {
                        break;
                    }
                    this.position = this._lastPositions[i];
                    if (this.owner != null)
                    {
                        this.position += this.owner.velocity;
                    }
                    base.depth -= 2;
                    base.alpha -= 0.15f;
                    base.graphic.color = Color.White;
                }
                this.position = position;
                base.depth = depth;
                base.alpha = 1f;
                this._angle = angle2;
                base.xscale = 1f;
                this._drawing = false;
            }
            else
            {
                base.Draw();
            }
            this._lastAngles.Insert(0, this.angle);
            this._lastPositions.Insert(0, this.position);
            if (this._lastAngles.Count > 2)
            {
                this._lastAngles.Insert(0, (this._lastAngles[0] + this._lastAngles[2]) / 2f);
                this._lastPositions.Insert(0, (this._lastPositions[0] + this._lastPositions[2]) / 2f);
            }
            if (this._lastAngles.Count > 8)
            {
                this._lastAngles.RemoveAt(this._lastAngles.Count - 1);
            }
            if (this._lastPositions.Count > 8)
            {
                this._lastPositions.RemoveAt(this._lastPositions.Count - 1);
            }
        }

        public override void OnPressAction()
        {
            if ((this._crouchStance && this._jabStance && !this._swinging) || (!this._crouchStance && !this._swinging && this._swing < 0.1f))
            {
                this._pullBack = true;
                this._swung = true;
                this._shing = false;
                SFX.Play("swipe", Rando.Float(0.8f, 1f), Rando.Float(-0.1f, 0.1f), 0f, false);
                if (!this._jabStance)
                {
                    this._swordSwing.speed = 0.6f;
                    this._swordSwing.frame = 0;
                    return;
                }
                if (this._jabStance)
                {
                    float bulletAngle = (int)offDir >= 90 ? angleDegrees + _ammoType.barrelAngleDegrees : angleDegrees + -90f - _ammoType.barrelAngleDegrees;
                    Vec2 vec2_1 = this.Offset(this.barrelOffset);
                    Bullet bullet = ammoType.FireBullet(vec2_1, this.owner as Duck, bulletAngle, this);
                    if (Network.isActive && isServerForObject)
                        firedBullets.Add(bullet);
                    SFX.Play(_fireSound, 1f, Rando.Float(0.2f) - 0.1f, 0f, false);
                }
            }
            else if (this._crouchStance && !this._jabStance && base.duck != null && !base.duck.grounded)
            {
                this._slamStance = true;
            }
        }

        public override void Fire()
        {
        }
    }


//Fire
}
