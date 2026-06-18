using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Explosives")]
    public class holyHandGrenade : Gun
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
        private bool hallelujah = true;
        private Sprite _holyFlash;
        private float alpha2 = 0;

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

        public holyHandGrenade(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new ATShrapnel();
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("holygrenade"), 16, 16, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-4f, -5f);
            this.collisionSize = new Vec2(8f, 10f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this._holyFlash = new Sprite(GetPath("holyFlash"), 0.0f, 0.0f);
            this._holyFlash.CenterOrigin();
            this._holyFlash.alpha = 0.0f;
            this._holdOffset = new Vec2(0f, 1f);
            this._editorName = "Holy Hand Grenade";
            this.editorTooltip = "First shalt thou take out the Holy Pin, then thou shalt watch the light cleanse thine battlefield.";
        }

        public override void Initialize()
        {
            this.gr = holyHandGrenade.grenade;
            ++holyHandGrenade.grenade;
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
            /*
            int num1 = 8;
            if (Graphics.effectsLevel < 2)
                num1 = 3;
            for (int index = 0; index < num1; ++index)
            {
                float deg = (float)index * 60f + Rando.Float(-10f, 10f);
                float num2 = Rando.Float(20f, 26f);
                Level.Add((Thing)new ExplosionPart(x + (float)Math.Cos((double)Maths.DegToRad(deg)) * num2, ypos - (float)Math.Sin((double)Maths.DegToRad(deg)) * num2, true));
            }
            */
            this._explosionCreated = true;
            SFX.Play("explode", 1f, 0.0f, 0.0f, false);
        }

        public override void Update()
        {
            base.Update();
            if (!this._pin)
            {
                this._timer -= 0.01f;
                this._holyFlash.alpha = Lerp.Float(this._holyFlash.alpha, 0.8f, 0.002f);
                if (this._timer < -1f)
                {
                    alpha2 = Lerp.Float(alpha2, 0.8f, 0.01f);
                }
            }
            if ((double)this._timer < 0.5 && this.owner == null && !this._didBonus)
            {
                this._didBonus = true;
                if (Recorder.currentRecording != null)
                    Recorder.currentRecording.LogBonus();
            }
            if (!this._localDidExplode && (double)this._timer < -0.3 && this.hallelujah == true)
            {
                this.hallelujah = false;
                SFX.Play(GetPath("hallelujah"), 1.2f, Rando.Float(0f, 0.1f), 0.0f, false);
            }
            if (!this._localDidExplode && (double)this._timer < -1.5)
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
                        /*
                        float x = this.x;
                        float num1 = this.y - 2f;
                        */
                        Graphics.FlashScreen();
                        /*
                        if (this.isServerForObject)
                        {
                            for (int index = 0; index < 20; ++index)
                            {
                                float num2 = (float)((double)index * 18.0 - 5.0) + Rando.Float(10f);
                                ATShrapnel atShrapnel = new ATShrapnel();
                                atShrapnel.range = 60f + Rando.Float(18f);
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
                        */
                        Graphics.FlashScreen();
                        Vec2 pos = this.Offset(this.center - this.center);
                        Vec2 target = this.Offset(new Vec2(1200f, 0.0f)) - pos;
                        DeathBeam deathBeam = new DeathBeam(pos, target);
                        deathBeam.isLocal = this.isServerForObject;
                        Level.Add((Thing)deathBeam);

                        Vec2 target2 = this.Offset(new Vec2(-1200f, 0.0f)) - pos;
                        DeathBeam deathBeam2 = new DeathBeam(pos, target2);
                        deathBeam2.isLocal = this.isServerForObject;
                        Level.Add((Thing)deathBeam2);

                        Vec2 target3 = this.Offset(new Vec2(0.0f, 1200f)) - pos;
                        DeathBeam deathBeam3 = new DeathBeam(pos, target3);
                        deathBeam3.isLocal = this.isServerForObject;
                        Level.Add((Thing)deathBeam3);

                        Vec2 target4 = this.Offset(new Vec2(0.0f, -1200f)) - pos;
                        DeathBeam deathBeam4 = new DeathBeam(pos, target4);
                        deathBeam4.isLocal = this.isServerForObject;
                        Level.Add((Thing)deathBeam4);

                        this._holyFlash.alpha = Lerp.Float(this._holyFlash.alpha, 0f, 0.08f);
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
            GrenadePin grenadePin = new GrenadePin(this.x, this.y);
            grenadePin.hSpeed = (float)-this.offDir * (1.5f + Rando.Float(0.5f));
            grenadePin.vSpeed = -2f;
            Level.Add((Thing)grenadePin);
            SFX.Play("pullPin", 1f, 0.0f, 0.0f, false);
        }

        public override void Draw()
        {
            if (this._timer < 0.04f)
            {
                Graphics.DrawLine(this.Offset(this.center - this.center), this.Offset(new Vec2(0f, 1200f)), Color.Yellow * (alpha2), 32f, (Depth)0.9f);
                Graphics.DrawLine(this.Offset(this.center - this.center), this.Offset(new Vec2(1200f, 0f)), Color.Yellow * (alpha2), 32f, (Depth)0.9f);
                Graphics.DrawLine(this.Offset(this.center - this.center), this.Offset(new Vec2(0.0f, -1200f)), Color.Yellow * (alpha2), 32f, (Depth)0.9f);
                Graphics.DrawLine(this.Offset(this.center - this.center), this.Offset(new Vec2(-1200f, 0f)), Color.Yellow * (alpha2), 32f, (Depth)0.9f);
                Graphics.DrawLine(this.Offset(this.center - this.center), this.Offset(new Vec2(0f, 1200f)), Color.White * (alpha2), 22f, (Depth)0.9f);
                Graphics.DrawLine(this.Offset(this.center - this.center), this.Offset(new Vec2(1200f, 0f)), Color.White * (alpha2), 22f, (Depth)0.9f);
                Graphics.DrawLine(this.Offset(this.center - this.center), this.Offset(new Vec2(0.0f, -1200f)), Color.White * (alpha2), 22f, (Depth)0.9f);
                Graphics.DrawLine(this.Offset(this.center - this.center), this.Offset(new Vec2(-1200f, 0f)), Color.White * (alpha2), 22f, (Depth)0.9f);
            }
            if (!this._pin)
            {
                Graphics.DrawLine(this.Offset(this.center - this.center), this.Offset(new Vec2(0.0f, 1200f)), Color.Yellow * (this._holyFlash.alpha), 2f, (Depth)0.9f);
                Graphics.DrawLine(this.Offset(this.center - this.center), this.Offset(new Vec2(1200f, 0f)), Color.Yellow * (this._holyFlash.alpha), 2f, (Depth)0.9f);
                Graphics.DrawLine(this.Offset(this.center - this.center), this.Offset(new Vec2(0.0f, -1200f)), Color.Yellow * (this._holyFlash.alpha), 2f, (Depth)0.9f);
                Graphics.DrawLine(this.Offset(this.center - this.center), this.Offset(new Vec2(-1200f, 0f)), Color.Yellow * (this._holyFlash.alpha), 2f, (Depth)0.9f);
            }
            if ((double)this._holyFlash.alpha > 0.00999999977648258)
                Graphics.Draw(this._holyFlash, this.x, this.y);
            base.Draw();
        }
        }
}