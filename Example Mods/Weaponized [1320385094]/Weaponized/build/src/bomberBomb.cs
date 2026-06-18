using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Explosives")]
    public class bomberBomb : Gun
    {
        public StateBinding _timerBinding = new StateBinding("_timer", -1, false, false);
        public StateBinding _pinBinding = new StateBinding("_pin", -1, false, false);
        public bool _pin = true;
        public float _timer = 1.2f;
        public int _explodeFrames = -1;
        private SpriteMap _sprite;
        private Duck _cookThrower;
        private float _cookTimeOnThrow;
        public bool pullOnImpact;
        private bool _explosionCreated;
        private bool _localDidExplode;
        private bool _didBonus;
        private static int grenade;
        public int gr;
        private Sound _burnSound;

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

        public bomberBomb(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new ATShrapnel();
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("bomberman"), 10, 12, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(5f, 7f);
            this.collisionOffset = new Vec2(-5f, -7f);
            this.collisionSize = new Vec2(10f, 12f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this._sprite.AddAnimation("before", 0.2f, true, 0);
            this._sprite.AddAnimation("used", 0.15f, false, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10);
            this._sprite.SetAnimation("before");
            this._editorName = "Bomber Bomb";
            this.editorTooltip = "This bomb can explode again and again and again...";
        }

        public override void Initialize()
        {
            this.gr = bomberBomb.grenade;
            ++bomberBomb.grenade;
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
            this.angleDegrees = 0.0f;
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
                float deg = (float)index * 60f + Rando.Float(-10f, 10f);
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
                Vec2 vec2_i = this.Offset(new Vec2(0f, -5f));
                Level.Add((Thing)Spark.New(vec2_i.x, vec2_i.y, new Vec2(Rando.Float(-1f, 1f), -0.5f), 0.1f));
                this._timer -= 0.01f;
            }
            if ((double)this._timer < 0.5 && this.owner == null && !this._didBonus)
            {
                this._didBonus = true;
                if (Recorder.currentRecording != null)
                    Recorder.currentRecording.LogBonus();
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
                        if (this._burnSound != null)
                        {
                            this._burnSound.Stop();
                            this._burnSound = (Sound)null;
                        }
                        if (this.isServerForObject)
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                for (int index = 0; index < 20; ++index)
                                {
                                    float num2 = (float)((double)index * 18.0 - 5.0) + Rando.Float(10f);
                                    ATShrapnel atShrapnel = new ATShrapnel();
                                    atShrapnel.range = 40f + Rando.Float(10f);
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
                        }
                        if (this.isServerForObject)
                        {
                            Vec2 vec2_b = this.Offset(new Vec2(0f, 0f));
                            bomberBomb bBomb = new bomberBomb(vec2_b.x, vec2_b.y);
                            bBomb.hSpeed = Rando.Float(-2f, 2f);
                            bBomb.vSpeed = -3.5f + Rando.Float(0f, -1f);
                            Level.Add((Thing)bBomb);
                        }
                        this._explodeFrames = -1;
                        Level.Remove((Thing)this);
                        /*this._explosionCreated = false;
                        this._pin = true;
                        this._timer = 1.2f;
                        this._sprite.SetAnimation("before");*/
                    }
                }
            }
            if (this.prevOwner != null && this._cookThrower == null)
            {
                this._cookThrower = this.prevOwner as Duck;
                this._cookTimeOnThrow = this._timer;
            }
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this.pullOnImpact)
                this.OnPressAction();
            base.OnSolidImpact(with, from);
        }

        public override void OnPressAction()
        {
            this._burnSound = SFX.Play("fuseBurn", 0.5f, 0.0f, 0.0f, false);
            this._sprite.SetAnimation("used");
            if (!this._pin)
                return;
            this._pin = false;
            SFX.Play("lightMatch", 1f, 0.0f, 0.0f, false);
        }
    }
}
