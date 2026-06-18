using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Slo-Mo")]

    public class slomoGrenade : Gun
    {
        public StateBinding _timerBinding = new StateBinding("_timer", -1, false, false);
        public StateBinding _pinBinding = new StateBinding("_pin", -1, false, false);
        public bool _pin = true;
        public float _timer = 2.4f;
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

        public slomoGrenade(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new slomoATShrapnel();
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("slomoGrenade"), 16, 16, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-4f, -5f);
            this.collisionSize = new Vec2(8f, 10f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this._editorName = "Slo-Mo Grenade";
            this.editorTooltip = "These bullets are so slooow!";
        }

        public override void Initialize()
        {
            this.gr = slomoGrenade.grenade;
            ++slomoGrenade.grenade;
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
                this._timer -= 0.01f;
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
                        if (this.isServerForObject)
                        {
                            for (int index = 0; index < 30; ++index)
                            {
                                float num2 = (float)((double)index * 18.0 - 5.0) + Rando.Float(10f);
                                slomoATShrapnel slomoatShrapnel = new slomoATShrapnel();
                                slomoatShrapnel.range = 130f + Rando.Float(20f);
                                Bullet bullet = new Bullet(x + (float)(Math.Cos((double)Maths.DegToRad(num2)) * 6.0), num1 - (float)(Math.Sin((double)Maths.DegToRad(num2)) * 6.0), (AmmoType)slomoatShrapnel, num2, (Thing)null, false, -1f, false, true);
                                bullet.firedFrom = (Thing)this;
                                this.firedBullets.Add(bullet);
                                Level.Add((Thing)bullet);
                            }
                            foreach (Window window in Level.CheckCircleAll<Window>(this.position, 40f))
                            {
                                if (Level.CheckLine<Block>(this.position, window.position, (Thing)window) == null)
                                    window.Destroy((DestroyType)new DTImpact((Thing)this));
                            }
                            this.bulletFireIndex += (byte)30;
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
            slomoShell slomoShell = new slomoShell(this.x, this.y);
            slomoShell.hSpeed = (float)-this.offDir * (1.5f + Rando.Float(0.5f));
            slomoShell.vSpeed = -2f;
            Level.Add((Thing)slomoShell);
            SFX.Play("pullPin", 1f, 0.0f, 0.0f, false);
        }
    }
}
