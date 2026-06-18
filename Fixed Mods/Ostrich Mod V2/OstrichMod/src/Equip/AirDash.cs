using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    [EditorGroup("OstrichMod | Tech")]
    internal class AirDash : Boots
    {
        private float _bulletOffset = 20f;
        private float _fireClock = 0.0f;
        private float _fireDelay = 0.05f;
        private bool _justShot = false;
        private bool _canShoot = true;
        private int ammo;
        private AmmoType _ammoType;

        public AirDash(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._editorName = "Air Dash";
            this._pickupSprite = new Sprite(this.GetPath("AirDashPickup"), 0.0f, 0.0f);
            this._sprite = new SpriteMap(this.GetPath("AirDash"), 32, 32, false);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -6f);
            this.collisionSize = new Vec2(12f, 13f);
            this._equippedDepth = 1;
            this.ammo = 2;
            this._ammoType = (AmmoType)new ATSniper();
            this._jumpMod = true;
        }

        public virtual void Shoot()
        {
            if (this._equippedDuck == null)
                return;
            this.ammo = this.ammo - 1;
            SFX.Play(this.GetPath("AirDash"), 1f, 0.0f, 0.0f, false);
            if (this._equippedDuck.inputProfile.Down("LEFT"))
                this._equippedDuck.velocity = new Vec2(this._equippedDuck.velocity.x - 5f, this._equippedDuck.velocity.y - 0f);
            if (this._equippedDuck.inputProfile.Down("RIGHT"))
                this._equippedDuck.velocity = new Vec2(this._equippedDuck.velocity.x + 5f, this._equippedDuck.velocity.y - 0f);
            if (this._equippedDuck.inputProfile.Down("UP"))
                this._equippedDuck.velocity = new Vec2(this._equippedDuck.velocity.x - 0f, this._equippedDuck.velocity.y - 5f);
            if (this._equippedDuck.inputProfile.Down("DOWN"))
                this._equippedDuck.velocity = new Vec2(this._equippedDuck.velocity.x - 0f, this._equippedDuck.velocity.y + 5f);
        }

        public override void Update()
        {
            base.Update();
            if (this._justShot)
            {
                this._fireClock = this._fireClock + 0.01f;
                this._canShoot = false;
            }
            if ((double)this._fireClock >= (double)this._fireDelay)
            {
                this._fireClock = 0.0f;
                this._justShot = false;
                this._canShoot = true;
            }
            if (this._equippedDuck == null)
                return;
            this._bulletOffset = !this._equippedDuck.crouch ? 20.5f : 30f;
            if (this._equippedDuck.inputProfile.Pressed("JUMP", false) && this.ammo != 0 && ((double)this._equippedDuck.framesSinceJump > 3.0 && this._equippedDuck.ragdoll == null) && !this._equippedDuck.sliding && this._canShoot)
            {
                this.Shoot();
                this._justShot = true;
            }
            if (this._equippedDuck.grounded && this.ammo < 1)
            {
                this.ammo = 2;
                SFX.Play(this.GetPath("AirDashReload"), 1f, 0.0f, 0.0f, false);
            }
        }
    }
}