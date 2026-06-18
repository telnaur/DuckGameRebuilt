using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Western")]
    class OldRevolver : Gun
    {
        public StateBinding _burstingBinding = new StateBinding("_bursting", -1, false);

        public StateBinding _burstNumBinding = new StateBinding("_burstNum", -1, false);

        public float _burstWait;

        public bool _bursting;

        public int _burstNum;

        public OldRevolver(float xval, float yval) : base(xval, yval)
        {
            this._editorName = "Old Revolver";
            this.ammo = 9;
            this._ammoType = (AmmoType)new ATMagnum();
            this._ammoType.accuracy = 0.8f;
            this._ammoType.penetration = 1f;
            this._ammoType.range = 100f;
            this._fireWait = 4f;
            this._type = "gun";
            this.graphic = new SpriteMap(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("OldRevolver"), 22, 11);
            this.center = new Vec2(11f, 6f);
            this.collisionOffset = new Vec2(-11f, -6f);
            this.collisionSize = new Vec2(22f, 11f);
            this._barrelOffsetTL = new Vec2(23f, 3f);
            this._fireSound = GetPath("sounds/musky");
            this._kickForce = 0.3f;
            this._holdOffset = new Vec2(1f, 2f);
            this.handOffset = new Vec2(0.0f, 1f);
            this.loseAccuracy = 0.2f;
            this.maxAccuracyLost = 0.5f;
        }

        public override void Update()
        {
            if (this._bursting)
            {
                this._burstWait = Maths.CountDown(this._burstWait, 0.1f, 0.0f);
                if ((double)this._burstWait <= 0.0)
                {
                    this._burstWait = 0.8f;
                    if (this.isServerForObject)
                    {
                        this.Fire();
                        Send.Message((NetMessage)new NMFireGun((Gun)this, this.firedBullets, this.bulletFireIndex, false, this.duck != null ? this.duck.netProfileIndex : (byte)4, true), NetMessagePriority.Urgent, (NetworkConnection)null);
                        this.firedBullets.Clear();
                    }
                    this._wait = 0.0f;
                    ++this._burstNum;
                }
                if (this._burstNum == 3)
                {
                    this._burstNum = 0;
                    this._burstWait = 0.0f;
                    this._bursting = false;
                    this._wait = this._fireWait;
                }
            }
            base.Update();
        }

        public override void OnPressAction()
        {
            if (this.receivingPress && this.hasFireEvents && this.onlyFireAction)
                this.Fire();
            if (this._bursting || (double)this._wait != 0.0)
                return;
            this._bursting = true;
            SmallSmoke smallSmoke = SmallSmoke.New(this.barrelPosition.x, this.barrelPosition.y);
            smallSmoke.scale = new Vec2(0.3f, 0.3f);
            smallSmoke.hSpeed = Rando.Float(-0.1f, 0.1f);
            smallSmoke.vSpeed = -Rando.Float(0.05f, 0.2f);
            smallSmoke.alpha = 0.6f;
            Level.Add((Thing)smallSmoke);
        }

        public override void OnHoldAction()
        {
        }

    }
}