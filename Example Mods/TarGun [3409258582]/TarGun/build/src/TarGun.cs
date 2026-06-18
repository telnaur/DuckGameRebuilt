using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.TarGunMod
{
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("isFatal", false)]
    public class TarGun : FlareGun
    {
        public StateBinding _nextShotProjCountBinding = new StateBinding("projCount");
        public int projCount;


        protected SpriteMap _loaderSprite;
        private readonly SpriteMap _sprite;
        public TarGun(float xval, float yval)
            : base(xval, yval)
        {
            this.isFatal = false;
            this._sprite = new SpriteMap(GetPath("tarGun4"), 16, 12);
            this.graphic = this._sprite;
            this._editorName = "Tar Gun";
            this.editorTooltip = "Shoots blobs of sticky asphalt. Yuck.";
            this._bio = "All hail the industrial revolution!";
            this._ammoType.combustable = false;
            this.ammo = 4;
            this._kickForce = 3f;
        }
        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                projCount = Rando.Int(2, 4);
                --this.ammo;

                if ((bool)this.infinite)
                    this._sprite.frame = 0;
                else this._sprite.frame = 4 - this.ammo;

                SFX.Play("netGunFire", 0.5f, Rando.Float(0.2f) - 0.4f);

                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                this.ApplyKick();
                if (this.receivingPress || !this.isServerForObject)
                    return;

                if (isServerForObject)
                {
                    Vec2 projPos = this.Offset(this.barrelOffset);
                    for (int i = 0; i < projCount; i++)
                    {
                        Vec2 projDir = Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.075f, 0.075f) * projCount);
                        TarBlast projectile = new TarBlast(projPos.x, projPos.y, 1.5f - projCount * 0.125f, (int)(8 - projCount * 1.5f));
                        Fondle(projectile);
                        projectile.hSpeed = projDir.x * (9f - projCount/1.5f + Rando.Float(-1f, 1f));
                        projectile.vSpeed = projDir.y * (9f - projCount/1.5f + Rando.Float(-1f, 1f));
                        Level.Add(projectile);
                    }
                    for (int i = 0; i < 2+projCount; ++i)
                    {
                        Vec2 projDir = Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.3f, 0.3f) * projCount)
                            * new Vec2(Rando.Float(0.1f, 3f), Rando.Float(2f, 4f));
                        Level.Add(TarParticle.New(x - hSpeed, y - vSpeed, projDir.x, projDir.y, 1.25f - (projCount*0.125f)));
                    }
                }
            }
            else
            {
                this.DoAmmoClick();
            }
        }
    }
}