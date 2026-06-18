using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Misc|Offline")]
    [BaggedProperty("isOnlineCapable", false)]
    public class redEyeBean : Gun
    {
        private bool thrown = false;
        public redEyeBean(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 99;
            this._ammoType = (AmmoType)new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._type = "gun";

            this.graphic = new Sprite(GetPath("redEyeBean"), 0.0f, 0.0f);
            this.center = new Vec2(4f, 4f);
            this.collisionOffset = new Vec2(-4f, -4f);
            this.collisionSize = new Vec2(7f, 7f);
            this._holdOffset = new Vec2(-1f, 2f);
            this.weight = 2f;

            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this.flammable = 0.8f;
            this._editorName = "Laser Eye Bean";
            this.editorTooltip = "These beans must have come from a different planet, we don't have laser vegetation on Earth yet.";
        }

        public override void Update()
        {
            if (this.thrown && this.grounded)
            {
                Level.Add((Thing)SmallSmoke.New(this.x, this.y));
                redEyeFlower redEyeFlower = new redEyeFlower(this.x, this.y + 2f);
                if (Network.isActive)
                {
                    redEyeFlower.Fondle((Thing)redEyeFlower);
                }
                Level.Add((Thing)redEyeFlower);
                SFX.Play("littleSplash", 0.7f, Rando.Float(0.2f) - 0.4f, 0.0f, false);
                Level.Remove((Thing)this);
            }
            base.Update();
        }

        public override void Fire()
        {
        }

        public override void OnPressAction()
        {
            Duck owner = this.owner as Duck;
            if (owner != null)
            {
                owner.doThrow = true;
                this.thrown = true;
            }
        }
    }
}