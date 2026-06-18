using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Misc")]
    [BaggedProperty("isFatal", false)]

    public class potion : Gun
    {
          
    public StateBinding _bananaStateBinding = (StateBinding)new StateFlagBinding(new string[2]
    {
      "_pin",
      "_thrown"
    });
        public StateBinding _netSplatBinding = (StateBinding)new NetSoundBinding("_netSplat");
        public NetSoundEffect _netSplat = new NetSoundEffect(new string[1] { "bulletHitWater" });
        public bool _pin = true;
        private SpriteMap _sprite;
        public bool _thrown;
        private bool _fade;
        private bool _splatted;

        public bool pin
        {
            get
            {
                return this._pin;
            }
        }



        public potion(float xval, float yval)
      : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = (AmmoType)new ATShrapnel();
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("potion"), 12, 11, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(6f, 6f);
            this.collisionOffset = new Vec2(-6f, -6f);
            this.collisionSize = new Vec2(12f, 11f);
            this._holdOffset = new Vec2(-1f, 2f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this.physicsMaterial = PhysicsMaterial.Crust;
            this._editorName = "Potion";
            this.editorTooltip = "Carefully engineered to shrink ducks down to exactly 55%.";
        }

        public override void EditorAdded()
        {
            this._sprite.SetAnimation("empty");
            base.EditorAdded();
        }

        public override void Update()
        {
            base.Update();
            if (this._thrown && this.owner == null)
            {
                this._thrown = false;
                if ((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed) > 0.400000005960464)
                    this.angleDegrees = 90f;
            }
            if (!this._pin && this.owner == null && !this._fade)
            {
                this._sprite.frame = 2;
                this.weight = 0.1f;
            }
            if (this._fade)
            {
                this.alpha -= 0.1f;
                if ((double)this.alpha <= 0.0)
                {
                    Level.Remove((Thing)this);
                    this.alpha = 0.0f;
                }
            }
            if (!this._pin && this.owner == null)
                this.canPickUp = false;
            if (!this._pin && this._grounded && !this._fade)
            {
                if (!this._splatted)
                {
                    this._splatted = true;
                    SFX.Play("glassBreak", 1f, Rando.Float(-0.2f, 0.2f), 0.0f, false);
                }
                this.angleDegrees = 0f;
                this.canPickUp = false;
                for (int index = 0; index < 6; ++index)
                {
                    potionDebris thing = potionDebris.New(this.x - 4f + Rando.Float(10f), this.y - 8f + Rando.Float(8f));
                    thing.hSpeed = (float)(((double)Rando.Float(1f) > 0.5 ? 1.0 : -1.0) * (double)Rando.Float(2f) + (double)Math.Sign(x) * 0.4);
                    thing.vSpeed = -Rando.Float(1f);
                    Level.Add((Thing) thing);
                }
                Level.Remove((Thing)this);
            }
                
            
            if (this._triggerHeld)
            {
                if (this.duck == null)
                    return;
                this.duck.quack = 20;
                if ((int)this.offDir > 0)
                {
                    this.handAngle = -1.099557f;
                    this.handOffset = new Vec2(8f, -1f);
                    this._holdOffset = new Vec2(-1f, 10f);
                }
                else
                {
                    this.handAngle = 1.099557f;
                    this.handOffset = new Vec2(8f, -1f);
                    this._holdOffset = new Vec2(-1f, 10f);
                }
            }
            else
            {
                this.handAngle = 0.0f;
                this.handOffset = new Vec2(0.0f, 0.0f);
                this._holdOffset = new Vec2(-1f, 2f);
            }
        }

        public override void HeatUp(Vec2 location)
        {
        }

        public void EatBanana()
        {
            int num = 0;
            for (int index = 0; index < 8; ++index)
            {

                potionSmoke potionSmoke = new potionSmoke((float)((double)this.x - 16.0 + (double)Rando.Float(32f) + (double)this.offDir * 10.0), this.y - 16f + Rando.Float(32f));
                potionSmoke.depth = (Depth)((float)(0.899999976158142 + (double)index * (1.0 / 1000.0)));
                if (num < 10)
                    potionSmoke.move.x -= (float)this.offDir * Rando.Float(0.1f);

                Level.Add((Thing)potionSmoke);
                ++num;
            }
            if(this.duck != null)
            {
                if (duck.isServerForObject)
                {
                    this.connection = duck.connection;
                    this._connection = duck.connection;
                    potionEffect unstucker = new potionEffect(duck);
                    Level.Add((Thing)unstucker);
                    duck.Fondle(unstucker);
                }
                this.duck.duckSize = 0.55f;
                this.duck.duckWidth = 0.55f;
                this.duck.vSpeed -= 0.1f;
                this.duck.hSpeed += Rando.Float(-0.5f, 0.5f);
                this.duck.sleeping = false;
            }
            this._sprite.frame = 1;
            this._pin = false;
            this._holdOffset = new Vec2(-2f, 3f);
            this.collisionOffset = new Vec2(-4f, -2f);
            this.collisionSize = new Vec2(8f, 4f);
            this.weight = 0.01f;
            if (Network.isActive)
            {
                if (this.isServerForObject)
                    this._netSplat.Play(1f, Rando.Float(-0.6f, 0.6f));
            }
            else
                SFX.Play("bulletHitWater", 1f, Rando.Float(-0.6f, 0.6f), 0.0f, false);
            this.bouncy = 0.0f;
            this.friction = 0.3f;
        }

        public override void OnPressAction()
        {
            if (!this.pin)
                return;
            if (this.owner != null)
                this.EatBanana();
            else
                return;

        }

        public override void OnHoldAction()
        {
        }

        public override void OnReleaseAction()
        {
        }
    }
}

