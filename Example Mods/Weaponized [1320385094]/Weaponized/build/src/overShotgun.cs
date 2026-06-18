using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Shotguns")]
    public class overShotgun : Gun
    {
        public sbyte _loadProgress = 100;
        public float _loadAnimation = 1f;
        public StateBinding _loadProgressBinding = new StateBinding("_loadProgress", -1, false, false);
        protected SpriteMap _loaderSprite;
        private bool popped = false;

        public overShotgun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 2;
            this._ammoType = (AmmoType)new ATShotgun();
            this.ammoType.impactPower = 8f;
            this.ammoType.range = 140f;
            this.ammoType.rangeVariation = 20f;
            this.ammoType.accuracy = -1.6f;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("overShotgun"), 0.0f, 0.0f);
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(16f, 9f);
            this._barrelOffsetTL = new Vec2(30f, 14f);
            this._fireSound = "shotgunFire2";
            this._kickForce = 7f;
            this._numBulletsPerFire = 16;
            this._manualLoad = true;
            this._loaderSprite = new SpriteMap(GetPath("overShotgunLoader"), 8, 8, false);
            this._loaderSprite.center = new Vec2(4f, 4f);
            this._editorName = "Uber Shotgun";
            this.editorTooltip = "The first shot disintegrates enemies, the second one... not so much.";

        }

        public override void Update()
        {
            base.Update();

            if ((double)this._loadAnimation == -1.0)
            {
                SFX.Play("shotgunLoad", 1f, 0.0f, 0.0f, false);
                this._loadAnimation = 0.0f;
            }
            if ((double)this._loadAnimation >= 0.0)
            {
                if ((double)this._loadAnimation == 0.5 && this.ammo != 0)
                    this._ammoType.PopShell(this.x, this.y, (int)-this.offDir);
                if ((double)this._loadAnimation < 1.0)
                    this._loadAnimation += 0.1f;
                else
                    this._loadAnimation = 1f;
            }
            if ((int)this._loadProgress < 0)
                return;
            if ((int)this._loadProgress == 50)
                this.Reload(false);
            if ((int)this._loadProgress < 100)
                this._loadProgress += (sbyte)10;
            else
                this._loadProgress = (sbyte)100;

        }

        public override void OnPressAction()
        {

            if (this.loaded)
            {

                base.OnPressAction();
                this._loadProgress = (sbyte)-1;
                this._loadAnimation = -0.01f;
                if (this.popped == false)
                {
                    SFX.Play("balloonPop", 1f, 0.0f, 0.0f, false);
                    this.graphic = new Sprite(GetPath("overShotgun2"), 0.0f, 0.0f);
                    this._fireSound = "pelletgunBad";
                    this._kickForce = 2f;
                    this._numBulletsPerFire = 6;
                    this._ammoType = (AmmoType)new ATShotgunBad();
                    for (int j = 0; j < 3; j++)
                    {
                        Level.Add(SmallSmoke.New(this.x + this.collisionOffset.x + Rando.Float(-2f, 2f), this.y + this.collisionOffset.y + Rando.Float(-2f, 2f)));
                    }
                }
                this.popped = true;
            }
            else
            {
                if ((int)this._loadProgress != -1)
                    return;
                this._loadProgress = (sbyte)0;
                this._loadAnimation = -1f;
            }
        }

        public override void Draw()
        {
            base.Draw();
            Vec2 vec2 = new Vec2(13f, -2f);
            float num = (float)Math.Sin((double)this._loadAnimation * 3.14000010490417) * 3f;
            this.Draw((Sprite)this._loaderSprite, new Vec2(vec2.x - 8f - num, vec2.y + 4f), 1);
        }
    }
}
