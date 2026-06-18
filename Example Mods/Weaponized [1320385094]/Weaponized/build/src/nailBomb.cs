using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Explosives|Offline")]
    [BaggedProperty("isOnlineCapable", false)]
    public class nailBomb : Gun
    {
        public bool _pin = true;
        public float _timer = 1.2f;
        private SpriteMap _sprite;
        public bool blownUp;
        public bool _armed;
        public int _framesSinceArm;
        public int _framesSinceGround;
        public bool _thrown;
        private Sprite _mineFlash;
        private float prevAngle;

        private bool _laserExist;
        public bool visibleLaser;
        private Tex2D _laserTex;

        private Vec2 detectionRange;
        public List<PhysicsObject> thingsInRangeAtArm = new List<PhysicsObject>();

        public bool pin
        {
            get
            {
                return this._pin;
            }
        }

        public nailBomb(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new ATShrapnel();
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("nailBomb"), 18, 16, false);
            this._sprite.AddAnimation("pickup", 1f, true, new int[1]);
            this._sprite.AddAnimation("idle", 0.05f, true, 1, 2);
            this._sprite.SetAnimation("pickup");
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(9f, 8f);
            this.collisionOffset = new Vec2(-5f, -5f);
            this.collisionSize = new Vec2(10f, 9f);
            this._mineFlash = new Sprite(GetPath("firemineFlash"), 0.0f, 0.0f);
            this._mineFlash.CenterOrigin();
            this._mineFlash.alpha = 0.0f;
            this.bouncy = 0.0f;
            this.friction = 0.2f;
            this.visibleLaser = false;
            this._laserOffsetTL = new Vec2(9f, 8f);
            this._editorName = "Nail Bomb";
            this.editorTooltip = "Shake it. Throw it. Watch it pack nails into anyone who tries to cross it.";
        }

        public void Arm()
        {
            if (this._armed)
                return;
            this._armed = true;
            if (!this.isServerForObject)
                return;
            if (Network.isActive)
                NetSoundEffect.Play("minePullPin");
            else
                SFX.Play("pullPin", 1f, 0.0f, 0.0f, false);

            this.visibleLaser = true;
            ATTracer atTracer = new ATTracer();
            atTracer.range = 2000f;
            float ang = (this.angleDegrees - 90f) * -1f;
            Vec2 vec2 = this.Offset(this.laserOffset);
            atTracer.penetration = 0.4f;
            this.detectionRange = new Bullet(vec2.x, vec2.y, (AmmoType)atTracer, ang, this.owner, false, -1f, true, true).end;
            IEnumerable<PhysicsObject> physicsObjects = Level.CheckLineAll<PhysicsObject>(new Vec2(this.x, this.y), this.detectionRange);
            foreach (PhysicsObject physicsObject in physicsObjects)
            {
                thingsInRangeAtArm.Add(physicsObject);
            }
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (this._pin)
                return false;
            this.BlowUp();
            return true;
        }

        public void UpdatePinState()
        {
            if (!this._pin)
            {
                this.canPickUp = false;
                this._sprite.SetAnimation("idle");
                this.collisionOffset = new Vec2(-6f, -2f);
                this.collisionSize = new Vec2(12f, 3f);
                this.depth = (Depth)0.8f;
                this._hasOldDepth = false;
                this.thickness = 1f;
                this.center = new Vec2(9f, 14f);
            }
            else
            {
                this.canPickUp = true;
                this._sprite.SetAnimation("pickup");
                this.collisionOffset = new Vec2(-5f, -4f);
                this.collisionSize = new Vec2(10f, 8f);
                this.thickness = -1f;
            }
        }

        public override void Update()
        {
            if (!this.pin)
            {
                this.collisionOffset = new Vec2(-6f, -2f);
                this.collisionSize = new Vec2(12f, 3f);
            }
            base.Update();
            if (!this.pin && (double)Math.Abs(this.prevAngle - this.angle) > 0.100000001490116)
            {
                Vec2 vec2_1 = new Vec2(14f, 3f);
                Vec2 vec2_2 = new Vec2(-7f, -2f);
                Vec2 vec2_3 = new Vec2(4f, 14f);
                Vec2 vec2_4 = new Vec2(-2f, -7f);
                float num = (float)Math.Abs(Math.Sin((double)this.angle));
                this.collisionSize = vec2_1 * (1f - num) + vec2_3 * num;
                this.collisionOffset = vec2_2 * (1f - num) + vec2_4 * num;
                this.prevAngle = this.angle;
            }
            this.UpdatePinState();
            if (this._sprite.imageIndex == 2)
                this._mineFlash.alpha = Lerp.Float(this._mineFlash.alpha, 0.4f, 0.08f);
            else
                this._mineFlash.alpha = Lerp.Float(this._mineFlash.alpha, 0.0f, 0.08f);
            if (this._armed)
                this._sprite.speed = 2f;
            if (this._thrown && this.owner == null)
            {
                this.canPickUp = false;
                this._thrown = false;
                if ((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed) > 0.400000005960464)
                {
                    this.angleDegrees = 180f;
                }
            }
            if (!this._pin && this._grounded)
            {
                this.angleDegrees = 0f;
                if (!this._armed)
                {
                    if (this._framesSinceGround > 20)
                    {
                        this.angleDegrees = 0f;
                        this.Arm();
                    }
                    else
                        ++this._framesSinceGround;
                }
            }
            if (this._armed)
                ++this._framesSinceArm;
            if (!this._pin && this._grounded && this._framesSinceArm > 4)
            {
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
                IEnumerable<PhysicsObject> physicsObjects = Level.CheckLineAll<PhysicsObject>(new Vec2(this.x, this.y), this.detectionRange);
                foreach (PhysicsObject physicsObject in physicsObjects)
                {
                    if (!thingsInRangeAtArm.Contains(physicsObject))
                    {
                        this._timer = -2f;
                        SFX.Play("doubleBeep", 1f, 0.0f, 0.0f, false);
                        break;
                    }
                }
                if(_timer > 0)
                {
                    foreach (PhysicsObject physicsObject in thingsInRangeAtArm)
                    {
                        if (!physicsObjects.Contains(physicsObject))
                        {
                            this._timer = -2f;
                            SFX.Play("doubleBeep", 1f, 0.0f, 0.0f, false);
                            break;
                        }
                    }
                }
            }
            if ((double)this._timer < 0.0 && this.isServerForObject)
            {
                this._timer = 1f;
                this.BlowUp();
            }

        }

        public override void DoUpdate()
        {
            if (this.visibleLaser && this._laserTex == null)
            {
                this._laserTex = Content.Load<Tex2D>("pointerLaser");
            }
            base.DoUpdate();
        }

        public void BlowUp()
        {
            if (this.blownUp)
                return;
            this.MakeBlowUpHappen(this.position);
            this.blownUp = true;
            if (!this.isServerForObject)
                return;
            foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(this.position, 22f))
            {
                if (physicsObject != this)
                {
                    Vec2 vec2 = physicsObject.position - this.position;
                    float num1 = (float)(1.0 - (double)Math.Min(vec2.length, 22f) / 22.0);
                    float num2 = num1 * 4f;
                    vec2.Normalize();
                    physicsObject.hSpeed += num2 * vec2.x;
                    physicsObject.vSpeed += -5f * num1;
                    physicsObject.sleeping = false;
                    this.Fondle((Thing)physicsObject);
                }
            }
            float x = this.position.x;
            float y = this.position.y;
            for (int index = 0; index < 25; ++index)
            {
                float ang;
                ATnail atNail = new ATnail();
                if (index > 20)
                {
                    ang = Rando.Float(92f, 150f);
                    atNail.bulletSpeed = 7f;
                    atNail.range = 130f + Rando.Float(20f);
                }
                else if (index > 15)
                {
                    ang = Rando.Float(30f, 88f);
                    atNail.bulletSpeed = 7f;
                    atNail.range = 130f + Rando.Float(20f);
                }
                else
                {
                    ang = index * (2f/3f) + 85f + Rando.Float(-0.05f, 0.05f);
                    atNail.bulletSpeed = 10f;
                }
                Bullet bullet = new Bullet(x + Rando.Float(-2f, 2f), y, (AmmoType)atNail, ang, (Thing)null, false, -1f, false, true);
                bullet.firedFrom = (Thing)this;
                this.firedBullets.Add(bullet);
                Level.Add((Thing)bullet);
            }
            this.bulletFireIndex += (byte)22;

            if (Network.isActive && this.isServerForObject)
            {
                Send.Message((NetMessage)new NMFireGun((Gun)this, this.firedBullets, this.bulletFireIndex, false, (byte)4, false), NetMessagePriority.ReliableOrdered, (NetworkConnection)null);
                this.firedBullets.Clear();
            }
            if (Recorder.currentRecording != null)
                Recorder.currentRecording.LogBonus();
            Level.Remove((Thing)this);
        }

        public void MakeBlowUpHappen(Vec2 pos)
        {
            if (this.blownUp)
                return;
            this.blownUp = true;
            SFX.Play("explode", 1f, 0.0f, 0.0f, false);
            RumbleManager.AddRumbleEvent(pos, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium, RumbleType.Gameplay));
            Graphics.FlashScreen();
            float x = pos.x;
            float y = pos.y;
            Level.Add((Thing)new ExplosionPart(x, y, true));
            int num1 = 6;
            if (Graphics.effectsLevel < 2)
                num1 = 3;
            for (int index = 0; index < num1; ++index)
            {
                float deg = (float)index * 60f + Rando.Float(-10f, 10f);
                float num2 = Rando.Float(12f, 20f);
                Level.Add((Thing)new ExplosionPart(x + (float)Math.Cos((double)Maths.DegToRad(deg)) * num2, y - (float)Math.Sin((double)Maths.DegToRad(deg)) * num2, true));
            }
        }

        public override void OnNetworkBulletsFired(Vec2 pos)
        {
            this.MakeBlowUpHappen(pos);
            base.OnNetworkBulletsFired(pos);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && this.owner == null && (!this.canPickUp && (double)this._timer > 0.0))
            {
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
                this.BlowUp();
            }
            return false;
        }

        public override void Draw()
        {
            if ((double)this._mineFlash.alpha > 0.00999999977648258)
                Graphics.Draw(this._mineFlash, this.x, this.y - 3f);
            if (this.visibleLaser)
            {
                ATTracer atTracer = new ATTracer();
                atTracer.range = 2000f;
                float ang = (this.angleDegrees - 90f)* -1f;
                Vec2 vec2 = this.Offset(this.laserOffset);
                atTracer.penetration = 0.4f;
                atTracer.ownerSafety = 1;
                this._wallPoint = new Bullet(vec2.x, vec2.y, (AmmoType)atTracer, ang, this.owner, false, -1f, true, true).end;
                this._laserExist = true;
            }
            base.Draw();
        }

        public override void DrawGlow()
        {
            if (this.visibleLaser && (this._laserTex != null && this._laserExist))
            {
                float num = 1f;
                if (!Options.Data.fireGlow)
                    num = 0.4f;
                Vec2 p1 = this.Offset(this.laserOffset);
                float length = (p1 - this._wallPoint).length;
                float val1 = 100f;
                if (this.ammoType != null)
                    val1 = this.ammoType.range;
                Vec2 normalized = (this._wallPoint - p1).normalized;
                Vec2 vec2 = p1 + normalized * Math.Min(val1, length);
                Graphics.DrawTexturedLine(this._laserTex, p1, vec2, Color.Red * num, 0.5f, this.depth - 1);
                if ((double)length > (double)val1)
                {
                    for (int index = 1; index < 4; ++index)
                    {
                        Graphics.DrawTexturedLine(this._laserTex, vec2, vec2 + normalized * 2f, Color.Red * (float)(1.0 - (double)index * 0.200000002980232) * num, 0.5f, this.depth - 1);
                        vec2 += normalized * 2f;
                    }
                }
                if (this._sightHit != null && (double)length < (double)val1)
                {
                    this._sightHit.alpha = num;
                    this._sightHit.color = Color.Red * num;
                    Graphics.Draw(this._sightHit, this._wallPoint.x, this._wallPoint.y);
                }
            }
            base.DrawGlow();
        }

        public override void OnPressAction()
        {
            if (!this.isServerForObject)
                return;
            if (this.owner == null)
            {
                this._pin = false;
                if ((double)this.heat > 0.5)
                    this.BlowUp();
            }
            if (!this._pin)
                return;
            this._pin = false;
            this.UpdatePinState();
            Duck owner = this.owner as Duck;
            if (owner != null)
            {
                owner.doThrow = true;
                this._responsibleProfile = owner.profile;
            }
            else
                this.Arm();
            this._thrown = true;
        }
    }
}

