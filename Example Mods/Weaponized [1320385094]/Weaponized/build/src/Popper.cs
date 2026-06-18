using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [BaggedProperty("isFatal", false)]
    [EditorGroup("Zyrafa|Guns|Fire")]

    public class popper : Gun
    {

        private bool pin = false;
        private bool pin2 = false;
        public popper(float xval, float yval)
          : base(xval, yval)

        {

            this._type = "gun";
            this.graphic = new Sprite(GetPath("partyPopper"), 0.0f, 0.0f);
            this.center = new Vec2(5f, 6f);
            this.collisionOffset = new Vec2(-5f, -6f);
            this.collisionSize = new Vec2(10f, 12f);
            this._fireSound = "smg";
            this._fireWait = 1f;
            this._kickForce = 3f;
            this._holdOffset = new Vec2(1f, 1f);
            this._barrelOffsetTL = new Vec2(0f, 0f);
            this.ammo = 1;
            this._editorName = "Firefighting Popper";
            this.editorTooltip = "1. Investigate the fire 2. pull the string 3. Drop and roll in the foam.";
        }

        public override void Update()
        {
            base.Update();
            if (this.pin2 == false)
                if (this.owner == null)
                    this.graphic = new Sprite(GetPath("partyPopper"), 0.0f, 0.0f);
                else
                    this.graphic = new Sprite(GetPath("partyPopper"), 0.0f, 0.0f);
        }

        public override void OnPressAction()
        {
            if (this.pin == false)
            {
                this.graphic = new Sprite(GetPath("partyPopper2"), 0.0f, 0.0f);
            }
            this.pin2 = true;
        }
        public override void OnReleaseAction()
        {
            if (this.pin == false)
            {
                for (int index = 0; index < 10; ++index)
                    Level.Add((Thing)Spark.New(this.x, this.y - 2f, new Vec2(Rando.Float(-1f, 1f), -0.5f), 0.1f));
                for (int index = 0; index < 14; ++index)
                {

                    ExtinguisherSmoke extinguisherSmoke = new ExtinguisherSmoke(x, y);
                    float num2 = (float)index / 5f;
                    extinguisherSmoke.hSpeed = (float)((double)num2 * 5.0 - 2.5) * Rando.Float(-0.3f, 0.3f);
                    extinguisherSmoke.vSpeed = Rando.Float(-0.5f, -2.5f);
                    Level.Add((Thing)extinguisherSmoke);


                }
                SFX.Play("deepMachineGun2", 1f, 0.0f, 0.0f, false);
                this.graphic = new Sprite(GetPath("partyPopper3"), 0.0f, 0.0f);
                this.pin = true;
                this.ammo = 0;
            }
        }


        public override void Fire()
        {
        }
    }
}
