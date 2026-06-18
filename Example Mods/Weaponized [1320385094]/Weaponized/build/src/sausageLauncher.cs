using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Guns|Fire")]
    public class sausageLauncher : Gun
    {
        private SpriteMap _barrelSteam;
        private SpriteMap _netGunGuage;

        public sausageLauncher(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = (AmmoType)new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._ammoType.penetration = -1f;
            this._ammoType.combustable = true;
            this._type = "gun";
            this.graphic = new Sprite(GetPath("sausageLauncher"), 0.0f, 0.0f);
            this.center = new Vec2(11f, 7f);
            this.collisionOffset = new Vec2(-11f, -7f);
            this.collisionSize = new Vec2(21f, 14f);
            this._barrelOffsetTL = new Vec2(18f, 3f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 2f;
            this._kickForce = 4f;
            this._fireRumble = RumbleIntensity.Kick;
            this._netGunGuage = new SpriteMap("netGunGuage", 8, 8, false);
            this._barrelSteam = new SpriteMap("steamPuff", 16, 16, false);
            this._barrelSteam.center = new Vec2(0.0f, 14f);
            this._barrelSteam.AddAnimation("puff", 0.4f, false, 0, 1, 2, 3, 4, 5, 6, 7);
            this._barrelSteam.SetAnimation("puff");
            this._barrelSteam.speed = 0.0f;
            this._holdOffset = new Vec2(-3f, -2f);
            this._editorName = "Sausage Launcher";
            this.editorTooltip = "One sausage is not enough for your BBQ? What about 4 sausages?";
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            this._netGunGuage.frame = 4 - Math.Min(this.ammo + 1, 4);
            if ((double)this._barrelSteam.speed > 0.0 && this._barrelSteam.finished)
                this._barrelSteam.speed = 0.0f;
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            if ((double)this._barrelSteam.speed > 0.0)
            {
                this._barrelSteam.alpha = 0.6f;
                this.Draw((Sprite)this._barrelSteam, new Vec2(6f, -4f), 1);
            }
            this.Draw((Sprite)this._netGunGuage, new Vec2(0f, -3f), 1);
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                --this.ammo;
                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None, RumbleType.Gameplay));
                SFX.Play("netGunFire", 1f, 0.0f, 0.0f, false);
                this._barrelSteam.speed = 1f;
                this._barrelSteam.frame = 0;
                this.ApplyKick();
                Vec2 vec2 = this.Offset(this.barrelOffset);
                if (this.receivingPress)
                    return;
                sausage sausage = new sausage(vec2.x, vec2.y - 2f);
                Level.Add((Thing)sausage);
                this.Fondle((Thing)sausage);
                if (this.owner != null)
                    sausage.responsibleProfile = this.owner.responsibleProfile;
                sausage.OnPressAction();
                sausage.clip.Add(this.owner as MaterialThing);
                sausage.hSpeed = this.barrelVector.x * 5f;
                sausage.vSpeed = (float)((double)this.barrelVector.y * 7.0 - 7);
            }
            else
                this.DoAmmoClick();
        }

        public override void Fire()
        {
        }
    }
}
