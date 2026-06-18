using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{

    [EditorGroup("Zyrafa|Guns|Lasers")]
    public class seeker : Gun
    {
        private SpriteMap _sprite;
        public float magazineSide = 10f;


        public seeker(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 14;
            this._ammoType = (AmmoType)new ATSeeker();
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("seeker"), 22, 11, false);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(6f, 7f);
            this.collisionOffset = new Vec2(-5f, -7f);
            this.collisionSize = new Vec2(18f, 11f);
            this._barrelOffsetTL = new Vec2(21f, 3f);
            this._fireSound = "laserRifle";
            this._fireWait = 1.4f;
            this._fullAuto = true;
            this._kickForce = 2f;
            this._holdOffset = new Vec2(-3f, 2f);
            this.loseAccuracy = 0.15f;
            this.maxAccuracyLost = 0.75f;
            this._numBulletsPerFire = 2;
            this._sprite.frame = 0;
            this._flare = new SpriteMap("laserFlare", 16, 16, false);
            this._flare.center = new Vec2(0.0f, 8f);
            this._editorName = "Seeker";
            this.editorTooltip = "Its ricocheting lasers can be both a blessing and a curse.";

        }
        public override void Update()
        {
            if (this.offDir > 0)
                this.magazineSide = 10f;
            else
                this.magazineSide = -10f;
            base.Update();
        }

        public override void OnPressAction()
        {
            if (this.ammo == 1)
            {
                    this._sprite.frame = 1;
                    seekerMagazine seekerMagazine = new seekerMagazine(this.x += magazineSide, this.y);
                    seekerMagazine.hSpeed = (float)this.offDir * 0.2f;
                    seekerMagazine.vSpeed = 2f;
                    Level.Add((Thing)seekerMagazine);
                    SFX.Play("pullPin", 1f, 0.0f, 0.0f, false);
            }
            this.Fire();
        }
    }
}