using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Lasers")]
    public class rickGun : Gun
    {
        private bool sound = true;
        private float timer = 2.2f;
        public rickGun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 18;
            this._ammoType = (AmmoType)new ATblueLaser();
            this._type = "gun";
            this.graphic = new Sprite(GetPath("rickGun"), 0.0f, 0.0f);
            this.center = new Vec2(10f, 6f);
            this.collisionOffset = new Vec2(-10f, -6f);
            this.collisionSize = new Vec2(19f, 11f);
            this._barrelOffsetTL = new Vec2(19f, 3f);
            this._fireSound = "laserRifle";
            this._kickForce = 1f;
            this._holdOffset = new Vec2(0.0f, 0.0f);
            this._flare = new SpriteMap(GetPath("redLaserFlare"), 16, 16, false);
            this._flare.center = new Vec2(0.0f, 8f);
            this._holdOffset = new Vec2(2f, 1f);
            this._fireWait = 3f;
            this._editorName = "Rick Gun";
            this.editorTooltip = "Due to the safety measures it starts shooting on its own when held.";
        }
        public override void Update()
        {
            if (this.owner != null)
            {
                if (sound == true)
                {
                    sound = false;
                    SFX.Play("stepInBeam", 0.7f, 0.0f, 0.0f, false);
                }
                timer -= 0.05f;
                if (this.ammo != 0 && timer <= 0)
                    this.PressAction();
            }
            else
            {
                sound = true;
                timer = 2.2f;
            }
            base.Update();
        }
    }
}