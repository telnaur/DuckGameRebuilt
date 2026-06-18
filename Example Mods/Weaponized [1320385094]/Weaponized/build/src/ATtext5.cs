using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{

    [BaggedProperty("canSpawn", false)]
    public class ATtext5 : Gun
    {
        public StateBinding _timerBinding = new StateBinding("_timer", -1, false, false);
        public StateBinding _pinBinding = new StateBinding("_pin", -1, false, false);
        public bool _pin = false;
        public float _timer = 1.2f;
        public float _dangerTimer = 0.1f;
        public int _explodeFrames = -1;
        public Duck ownerDuck;
        private bool _explosionCreated;
        private bool _localDidExplode;

        public ATtext5(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new ATShrapnel();
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            this.center = new Vec2(6f, 6f);
            this.collisionOffset = new Vec2(-4f, -4f);
            this.collisionSize = new Vec2(8f, 8f);
            this.bouncy = 1.1f;
            this.friction = 0.05f;
            this.gravMultiplier = 0.5f;
            this.frictionMult = 0.2f;
            this.physicsMaterial = PhysicsMaterial.Plastic;
            this.canPickUp = false;
        }

        public override void OnNetworkBulletsFired(Vec2 pos)
        {
            this._pin = false;
            this._localDidExplode = true;
            if (!this._explosionCreated)
                Graphics.FlashScreen();
            this.CreateExplosion(pos);
        }

        public void CreateExplosion(Vec2 pos)
        {
            if (this._explosionCreated)
                return;
            float x = pos.x;
            float ypos = pos.y - 2f;
            Level.Add((Thing)new ExplosionPart(x, ypos, true));
            this._explosionCreated = true;
            SFX.Play("explode", 0.8f, 0.0f, 0.0f, false);
        }
        public override void Touch(MaterialThing with)
        {
            if(with is Duck && (with != ownerDuck || _dangerTimer <= 0) || with is TrappedDuck || with is RagdollPart)
            {
                ownerDuck = null;
                with.Destroy((DestroyType)new DTImpale((Thing)this));
            }
        }

        public override void Fire()
        {
        }

        public override void Update()
        {
            base.Update();
            if(this._dangerTimer > 0)
            {
                this._dangerTimer -= 0.01f;
            }
            this.angleDegrees = 0f;
            if (!this._pin)
                this._timer -= 0.01f;
            if (!this._localDidExplode && (double)this._timer < 0.0)
            {
                if (this._explodeFrames < 0)
                {
                    this.CreateExplosion(this.position);
                    this._explodeFrames = 4;
                }
                else
                {
                    --this._explodeFrames;
                    if (this._explodeFrames == 0)
                    {
                        float x = this.x;
                        float num1 = this.y - 2f;
                        Graphics.FlashScreen();
                        if (this.isServerForObject)
                        {
                            for (int index = 0; index < 20; ++index)
                            {
                                float num2 = (float)((double)index * 18.0 - 5.0) + Rando.Float(10f);
                                ATShrapnel atShrapnel = new ATShrapnel();
                                atShrapnel.range = 10f + Rando.Float(10f);
                                Bullet bullet = new Bullet(x + (float)(Math.Cos((double)Maths.DegToRad(num2)) * 6.0), num1 - (float)(Math.Sin((double)Maths.DegToRad(num2)) * 6.0), (AmmoType)atShrapnel, num2, (Thing)null, false, -1f, false, true);
                                bullet.firedFrom = (Thing)this;
                                this.firedBullets.Add(bullet);
                                Level.Add((Thing)bullet);
                            }
                            this.bulletFireIndex += (byte)20;
                            if (Network.isActive)
                            {
                                Send.Message((NetMessage)new NMFireGun((Gun)this, this.firedBullets, this.bulletFireIndex, false, (byte)4, false), NetMessagePriority.ReliableOrdered, (NetworkConnection)null);
                                this.firedBullets.Clear();
                            }
                        }
                        Level.Remove((Thing)this);
                        this._destroyed = true;
                        this._explodeFrames = -1;
                    }
                }
            }
        }
    }
}