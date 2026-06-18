using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src

{
    [EditorGroup("Zyrafa|Guns|Fire")]
    [BaggedProperty("isFatal", false)]

    public class firemine : Gun
    {
        private int _numFlames;
        public StateBinding _pinBinding = new StateBinding("_pin", -1, false, false);
        public StateBinding _armedBinding = new StateBinding("_armed", -1, false, false);
        public StateBinding _clickedBinding = new StateBinding("_clicked", -1, false, false);
        public StateBinding _thrownBinding = new StateBinding("_thrown", -1, false, false);
        public StateBinding _netDoubleBeepBinding = (StateBinding)new NetSoundBinding("_netDoubleBeep");
        public NetSoundEffect _netDoubleBeep = new NetSoundEffect(new string[1] { "doubleBeep" });
        public StateBinding _netPinPlayHBinding = (StateBinding)new NetSoundBinding("_netPin");
        public NetSoundEffect _netPin = new NetSoundEffect(new string[1] { "pullPin" });
        public bool _pin = true;
        public float _timer = 1.2f;
        private Dictionary<Duck, float> _ducksOnMine = new Dictionary<Duck, float>();
        public List<PhysicsObject> previousThings = new List<PhysicsObject>();
        private SpriteMap _sprite;
        public bool blownUp;
        public bool _armed;
        public bool _clicked;
        public float addWeight;
        public int _framesSinceArm;
        public float _holdingWeight;
        public bool _thrown;
        private Sprite _mineFlash;
        private float prevAngle;

        public bool pin
        {
            get
            {
                return this._pin;
            }
        }

        public Dictionary<Duck, float> ducksOnMine
        {
            get
            {
                return this._ducksOnMine;
            }
        }

        public firemine(float xval, float yval, int numFlames = 14)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new ATShrapnel();
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("firemine"), 18, 16, false);
            this._sprite.AddAnimation("pickup", 1f, true, new int[1]);
            this._sprite.AddAnimation("idle", 0.05f, 1 != 0, 1, 2);
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
            this._numFlames = numFlames;
            this._editorName = "Fire Mine";
            this.editorTooltip = "Just like the regular mine, except a little bit more s p i c y.";
        }

        public void Arm()
        {
            if (this._armed)
                return;
            this._holdingWeight = 0.0f;
            this._armed = true;
            if (!this.isServerForObject)
                return;
            if (Network.isActive)
                this._netPin.Play(1f, 0.0f);
            else
                SFX.Play("pullPin", 1f, 0.0f, 0.0f, false);
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
                this._thrown = false;
                if ((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed) > 0.400000005960464)
                    this.angleDegrees = 180f;
            }
            if (this._armed)
                ++this._framesSinceArm;
            if (!this._pin && this._grounded && (!this._armed || this._framesSinceArm > 4))
            {
                this.canPickUp = false;
                float addWeight = this.addWeight;
                IEnumerable<PhysicsObject> physicsObjects = Level.CheckLineAll<PhysicsObject>(new Vec2(this.x - 6f, this.y - 3f), new Vec2(this.x + 6f, this.y - 3f));
                List<Duck> duckList1 = new List<Duck>();
                Duck duck = (Duck)null;
                bool flag = false;
                foreach (Thing previousThing in this.previousThings)
                {
                    if (previousThing.isServerForObject)
                        flag = true;
                }
                this.previousThings.Clear();
                foreach (PhysicsObject physicsObject in physicsObjects)
                {
                    if (physicsObject != this && physicsObject.owner == null && (!(physicsObject is Holdable) || (physicsObject as Holdable).canPickUp && (physicsObject as Holdable).hoverSpawner == null) && (double)Math.Abs(physicsObject.bottom - this.bottom) <= 6.0)
                    {
                        this.previousThings.Add(physicsObject);
                        if (physicsObject is Duck || physicsObject is TrappedDuck || physicsObject is RagdollPart)
                        {
                            addWeight += 5f;
                            Duck key = physicsObject as Duck;
                            if (physicsObject is TrappedDuck)
                                key = (physicsObject as TrappedDuck).captureDuck;
                            else if (physicsObject is RagdollPart && (physicsObject as RagdollPart).doll != null)
                                key = (physicsObject as RagdollPart).doll.captureDuck;
                            if (key != null)
                            {
                                duck = key;
                                if (!this._ducksOnMine.ContainsKey(key))
                                    this._ducksOnMine[key] = 0.0f;
                                Dictionary<Duck, float> ducksOnMine;
                                Duck index;
                                (ducksOnMine = this._ducksOnMine)[index = key] = ducksOnMine[index] + Maths.IncFrameTimer();
                                duckList1.Add(key);
                            }
                        }
                        else
                            addWeight += physicsObject.weight;
                    }
                }
                List<Duck> duckList2 = new List<Duck>();
                foreach (KeyValuePair<Duck, float> keyValuePair in this._ducksOnMine)
                {
                    if (!duckList1.Contains(keyValuePair.Key))
                        duckList2.Add(keyValuePair.Key);
                    else
                        keyValuePair.Key.profile.stats.timeSpentOnMines += Maths.IncFrameTimer();
                }
                foreach (Duck key in duckList2)
                    this._ducksOnMine.Remove(key);
                if ((double)addWeight < (double)this._holdingWeight && flag)
                {
                    Thing.Fondle((Thing)this, DuckNetwork.localConnection);
                    if (!this._armed)
                        this.Arm();
                    else
                        this._timer = -1f;
                }
                if (this._armed && (double)addWeight > (double)this._holdingWeight)
                {
                    if (!this._clicked && duck != null)
                        ++duck.profile.stats.minesSteppedOn;
                    this._clicked = true;
                    if (Network.isActive)
                        this._netDoubleBeep.Play(1f, 0.0f);
                    else
                        SFX.Play("doubleBeep", 1f, 0.0f, 0.0f, false);
                }
                this._holdingWeight = addWeight;
            }
            if ((double)this._timer < 0.0 && this.isServerForObject)
            {
                this._timer = 1f;
                this.BlowUp();
            }
            this.addWeight = 0.0f;
        }

        public void BlowUp()
        {
            if (this.blownUp)
                return;
            this.MakeBlowUpHappen(this.position);
            this.blownUp = true;
            foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(this.position, 22f))
            {
                if (physicsObject != this)
                {
                    Vec2 vec2 = physicsObject.position - this.position;
                    float num1 = (float)(1.0 - (double)Math.Min(vec2.length, 22f) / 22.0);
                    float num2 = num1 * 4f;
                    vec2.Normalize();
                    physicsObject.hSpeed += (float)((double)num2 * (double)vec2.x);
                    physicsObject.vSpeed += (float)(-5.0 * (double)num1);
                    physicsObject.sleeping = false;
                    this.Fondle((Thing)physicsObject);
                }
            }
            foreach (Duck duck in Level.CheckCircleAll<Duck>(this.position, 10f))
            {
                duck.Burn(duck.center, this);
            }
            float x = this.position.x;
            float y = this.position.y;
            for (int index = 0; index < this._numFlames; ++index)
                Level.Add((Thing)SmallFire.New(this.x, this.y, Rando.Float(-2.5f, 2.5f), Rando.Float(-2.5f, -0.5f), false, (MaterialThing)null, true, (Thing)this, false));
       
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
            SFX.Play("flameExplode", 1f, 0.0f, 0.0f, false);
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
            base.Draw();
        }

        public override void OnPressAction()
        {
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
                this._holdingWeight = 5f;
                owner.doThrow = true;
                this._responsibleProfile = owner.profile;
                this.Arm();
            }
            else
                this.Arm();
            this._thrown = true;
        }
    }
}
