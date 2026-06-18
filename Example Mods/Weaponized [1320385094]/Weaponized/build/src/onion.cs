using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Explosives")]

    public class onion : Gun
    {
        public StateBinding _timerBinding = new StateBinding("_timer", -1, false, false);
        public StateBinding _pinBinding = new StateBinding("_pin", -1, false, false);
        public bool _pin = true;
        public float _timer = 1.5f;
        public int _explodeFrames = -1;
        private SpriteMap _sprite;
        private Duck _cookThrower;
        private float _cookTimeOnThrow;
        public bool pullOnImpact;
        private bool _explosionCreated;
        private bool _localDidExplode;
        private bool _didBonus;
        private static int Onion;
        public int gr;
        public float _timer1 = 0.3f;
        public float _timer2 = 0.6f;
        public float _timer3 = 0.9f;
        public bool _pin1 = true;
        public bool _pin2 = true;
        public bool _pin3 = true;

        public Duck cookThrower
        {
            get
            {
                return this._cookThrower;
            }
        }

        public float cookTimeOnThrow
        {
            get
            {
                return this._cookTimeOnThrow;
            }
        }

        public onion(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new ATShrapnel();
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("onion"), 16, 16, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-4f, -6f);
            this.collisionSize = new Vec2(8f, 11f);
            this.bouncy = 0.5f;
            this.friction = 0.06f;
            this._editorName = "Onion Grenade";
            this.editorTooltip = "Creates lots of smelly smoke alongside the explosion.";
        }

        public override void Initialize()
        {
            this.gr = onion.Onion;
            ++onion.Onion;
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
            int num1 = 6;
            if (Graphics.effectsLevel < 2)
                num1 = 3;
            for (int index = 0; index < num1; ++index)
            {
                float deg = (float)index * 40f + Rando.Float(-10f, 10f);
                float num2 = Rando.Float(12f, 20f);
                Level.Add((Thing)new ExplosionPart(x + (float)Math.Cos((double)Maths.DegToRad(deg)) * num2, ypos - (float)Math.Sin((double)Maths.DegToRad(deg)) * num2, true));
            }
            this._explosionCreated = true;
            SFX.Play("explode", 1f, 0.0f, 0.0f, false);
        }

        public override void Update()
        {
            base.Update();
            if (!this._pin)
            {
                this._timer -= 0.01f;
                this._timer1 -= 0.01f;
                this._timer2 -= 0.01f;
                this._timer3 -= 0.01f;
            }
            if ((double)this._timer < 0.5 && this.owner == null && !this._didBonus)
            {
                this._didBonus = true;
                if (Recorder.currentRecording != null)
                    Recorder.currentRecording.LogBonus();
            }
            if (!this._localDidExplode && (double)this._timer1 < 0.0 && this._pin1 == true)
            { 
                int num = 0;
                for (int index = 0; index < 3; ++index)
                {

                    shrekSmoke shrekSmoke = new shrekSmoke((float)((double)this.x - 16.0 + (double)Rando.Float(32f) + (double)this.offDir * 10.0), this.y - 16f + Rando.Float(32f));
                    shrekSmoke.depth = (Depth)((float)(0.899999976158142 + (double)index * (1.0 / 1000.0)));
                    if (num < 5)
                        shrekSmoke.move.x -= (float)this.offDir * Rando.Float(0.1f);
                    Level.Add((Thing)shrekSmoke);
                    ++num;
                }
                SFX.Play("littleGun", 1f, 0.0f, 0.0f, false);
                this._pin1 = false;
            }
            if (!this._localDidExplode && (double)this._timer2 < 0.0 && this._pin2 == true)
            {
                int num = 0;
                for (int index = 0; index < 3; ++index)
                {

                    shrekSmoke shrekSmoke = new shrekSmoke((float)((double)this.x - 16.0 + (double)Rando.Float(32f) + (double)this.offDir * 10.0), this.y - 16f + Rando.Float(32f));
                    shrekSmoke.depth = (Depth)((float)(0.899999976158142 + (double)index * (1.0 / 1000.0)));
                    if (num < 5)
                        shrekSmoke.move.x -= (float)this.offDir * Rando.Float(0.1f);
                    Level.Add((Thing)shrekSmoke);
                    ++num;
                }
                SFX.Play("littleGun", 1f, 0.0f, 0.0f, false);
                this._pin2 = false;
            }
            if (!this._localDidExplode && (double)this._timer3 < 0.0 && this._pin3 == true)
            {
                int num = 0;
                for (int index = 0; index < 3; ++index)
                {

                    shrekSmoke shrekSmoke = new shrekSmoke((float)((double)this.x - 16.0 + (double)Rando.Float(32f) + (double)this.offDir * 10.0), this.y - 16f + Rando.Float(32f));
                    shrekSmoke.depth = (Depth)((float)(0.899999976158142 + (double)index * (1.0 / 1000.0)));
                    if (num < 5)
                        shrekSmoke.move.x -= (float)this.offDir * Rando.Float(0.1f);
                    Level.Add((Thing)shrekSmoke);
                    ++num;
                }
                SFX.Play("littleGun", 1f, 0.0f, 0.0f, false);
                this._pin3 = false;
            }
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
                                atShrapnel.range = 45f + Rando.Float(10f);
                                Bullet bullet = new Bullet(x + (float)(Math.Cos((double)Maths.DegToRad(num2)) * 6.0), num1 - (float)(Math.Sin((double)Maths.DegToRad(num2)) * 6.0), (AmmoType)atShrapnel, num2, (Thing)null, false, -1f, false, true);
                                bullet.firedFrom = (Thing)this;
                                this.firedBullets.Add(bullet);
                                Level.Add((Thing)bullet);
                            }
                            foreach (Window window in Level.CheckCircleAll<Window>(this.position, 40f))
                            {
                                if (Level.CheckLine<Block>(this.position, window.position, (Thing)window) == null)
                                    window.Destroy((DestroyType)new DTImpact((Thing)this));
                            }
                            this.bulletFireIndex += (byte)20;
                            if (Network.isActive)
                            {
                                Send.Message((NetMessage)new NMFireGun((Gun)this, this.firedBullets, this.bulletFireIndex, false, (byte)4, false), NetMessagePriority.ReliableOrdered, (NetworkConnection)null);
                                this.firedBullets.Clear();
                            }

                        }
                        int num = 0;
                        for (int index = 0; index < 10; ++index)
                        {

                            bigShrekSmoke bigShrekSmoke = new bigShrekSmoke((float)((double)this.x + (double)Rando.Float(-50f, 50f)), this.y + Rando.Float(-40f, 10f));
                            bigShrekSmoke.depth = (Depth)((float)(0.899999976158142 + (double)index * (1.0 / 1000.0)));
                            if (num < 8)
                                bigShrekSmoke.move.x -= (float)this.offDir * Rando.Float(0.1f);
                            Level.Add((Thing)bigShrekSmoke);
                            ++num;
                        }
                        Level.Remove((Thing)this);
                        this._destroyed = true;
                        this._explodeFrames = -1;
                    }
                }
            }
            if (this.prevOwner != null && this._cookThrower == null)
            {
                this._cookThrower = this.prevOwner as Duck;
                this._cookTimeOnThrow = this._timer;
            }
            this._sprite.frame = this._pin ? 0 : 1;
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this.pullOnImpact)
                this.OnPressAction();
            base.OnSolidImpact(with, from);
        }

        public override void OnPressAction()
        {
            if (!this._pin)
                return;
            this._pin = false;
            onionPin onionPin = new onionPin(this.x, this.y);
            onionPin.hSpeed = (float)-this.offDir * (1.5f + Rando.Float(0.5f));
            onionPin.vSpeed = -2f;
            Level.Add((Thing)onionPin);
            SFX.Play("pullPin", 1f, 0.0f, 0.0f, false);
            int num = 0;
            for (int index = 0; index < 8; ++index)
            {

                shrekSmoke shrekSmoke = new shrekSmoke((float)((double)this.x - 16.0 + (double)Rando.Float(32f) + (double)this.offDir * 10.0), this.y - 16f + Rando.Float(32f));
                shrekSmoke.depth = (Depth)((float)(0.899999976158142 + (double)index * (1.0 / 1000.0)));
                if (num < 5)
                    shrekSmoke.move.x -= (float)this.offDir * Rando.Float(0.1f);
                Level.Add((Thing)shrekSmoke);
                ++num;
            }
        }
    }
}
