using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Misc")]
    public class trainHat : Gun
    {
        public bool owned = false;
        public bool whistle = true;
        private float timer = 6f;
        public bool trainRight = false;
        private SpriteMap _sprite;
        private SpriteMap _hat;

        public trainHat(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.ammo = 99;
            this._ammoType = (AmmoType)new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._type = "gun";

            this._sprite = new SpriteMap(GetPath("trainGun"), 16, 16, false);
            this._sprite.frame = 1;
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 12f);
            this.collisionOffset = new Vec2(-7f, -4f);
            this.collisionSize = new Vec2(14f, 8f);

            this._hat = new SpriteMap(GetPath("trainHat"), 32, 32, false);
            this._hat.center = new Vec2(16f, 16f);

            this._barrelOffsetTL = new Vec2(18f, 8f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this.flammable = 0.8f;
            this._editorName = "Train Hat";
            this.editorTooltip = "Become the train's conductor and let it grind other duck to shreds.";
        }

        public override void Update()
        {
            if (this.owner != null)
            {
                if (this.whistle == false)
                {
                    this.timer -= 0.01f;
                    this._sprite.frame = 2;
                }   
                if (timer <= 0f)
                {
                    this.whistle = true;
                    timer = 6f;
                    for (int index = 0; index < 4; ++index)
                        Level.Add((Thing)SmallSmoke.New(this.x + Rando.Float(-2f, 2f), this.y + 5f + Rando.Float(-2f, 2f)));
                    SFX.Play("harp", 0.8f, 0.0f, 0.0f, false);
                }
                if (this.whistle == true)
                {
                    this._sprite.frame = 0;
                }
            }
            else
            {
                this._sprite.frame = 1;
            }
            base.Update();
        }

        public override void OnPressAction()
        {
            if (this.owner != null)
            {
                if (this.whistle == true)
                {
                    if (this.offDir == -1)
                        trainRight = true;
                    //if (this.isServerForObject)
                    //{
                    Vec2 vector = new Vec2(1f * this.offDir, 0.0f /*+ Rando.Float(-0.03f, 0.03f)*/);
                    train train = new train(this.x - 1300f * this.offDir, this.y - 10f, vector);
                    Level.Add((Thing)train);
                    for (int index = 0; index < 4; ++index)
                        Level.Add((Thing)SmallSmoke.New(this.x + Rando.Float(-2f, 2f), this.y /*+ 5f + Rando.Float(-2f, 2f)*/));
                    if (Network.isActive)
                    {
                        train.Fondle((Thing)train);
                    }
                    this.whistle = false;
                    SFX.Play(GetPath("whistleTrainShort"), 0.8f, 0.0f, 0.0f, false);
                    /*trainPin trainPin = new trainPin(this.x, this.y);
                    trainPin.hSpeed = (float)this.offDir * (1.5f + Rando.Float(0.5f));
                    trainPin.vSpeed = -2f;
                    Level.Add((Thing)trainPin);*/
                    //}
                }
            }
        }

        public override void OnHoldAction()
        {
        }

        public override void Fire()
        {
        }

        public override void Draw()
        {
            base.Draw();
            if (this.owner == null || !(this.owner is Duck))
                return;
            Duck owner = this.owner as Duck;
            if (owner.HasEquipment(typeof(Hat)))
                return;
            this._hat.alpha = owner._sprite.alpha;
            this._hat.flipH = owner._sprite.flipH;
            this._hat.depth = owner.depth + 1;
            if (owner._sprite.imageIndex > 11 && owner._sprite.imageIndex < 14)
                this._hat.angleDegrees = owner._sprite.flipH ? 90f : -90f;
            else
                this._hat.angleDegrees = 0.0f;
            Vec2 hatPoint = DuckRig.GetHatPoint(owner._sprite.imageIndex);
            Graphics.Draw((Sprite)this._hat, owner.x + hatPoint.x * owner._sprite.flipMultH, owner.y + hatPoint.y * owner._sprite.flipMultV);
        }
    }
}
