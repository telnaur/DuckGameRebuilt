using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Lasers")]
    public class pumpBeast : Gun
    {

        private SpriteMap _sprite;

        public pumpBeast(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 10;
            this._ammoType = (AmmoType)new ATLaser();
            this._type = "gun";
            this._sprite = new SpriteMap(GetPath("pumpBeast"), 27, 11);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(14f, 6f);
            this.collisionOffset = new Vec2(-14f, -6f);
            this.collisionSize = new Vec2(27f, 11f);
            this._barrelOffsetTL = new Vec2(27f, 2f);
            this._fireSound = "laserRifle";
            this._kickForce = 2f;
            this._holdOffset = new Vec2(5f, 0.0f);
            this.ammoType.range = 275f;
            this.ammoType.accuracy = 0.9f;
            this._flare = new SpriteMap("laserFlare", 16, 16, false);
            this._flare.center = new Vec2(0.0f, 8f);
            this.ammoType.rangeVariation = 10f;
            this._editorName = "Pump Beast";
            this.ammoType.bulletLength = 40f;
            this.editorTooltip = "Starts off accurate as a sniper rifle, ends up murderous as a shotgun.";
        }

        public override void Fire()
        {
            base.Fire();
            this._fireSoundPitch -= 0.1f;
            if (this.ammoType.range > 100f)
            {
                this.ammoType.range -= 25f;
            }
            if (this.ammoType.accuracy >= -0.3f)
            {
                this.ammoType.accuracy -= 0.15f;
            }
            if (this._numBulletsPerFire <= 3 && this.ammoType.range < 210f)
            {
                this._numBulletsPerFire += 1;
                this.ammoType.rangeVariation += 10f;
            }
            if (this._numBulletsPerFire <= 5 && this.ammoType.range < 110f)
            {
                this._numBulletsPerFire += 1;
            }
        }

        public override void Update()
        {
            if (this.ammo >= 9)
            {
                this._sprite.frame = 0;
            }
            else if (this.ammo >= 7)
            {
                this._sprite.frame = 1;
            }
            else if (this.ammo >= 5)
            {
                this._sprite.frame = 2;
            }
            else if (this.ammo >= 3)
            {
                this._sprite.frame = 3;
            }
            else if (this.ammo >= 1)
            {
                this._sprite.frame = 4;
            }
            else if (this.ammo == 0)
            {
                this._sprite.frame = 5;
            }
            base.Update();
        }
    }
}
