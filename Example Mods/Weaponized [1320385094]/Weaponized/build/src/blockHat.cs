using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Equipment")]

    public class blockHat : Helmet
    {
        public NetSoundEffect _netTing = new NetSoundEffect(new string[1] { "ting2" });

        private bool spawnedItem = false;
        protected Sprite _pickupEmptySprite;

        public blockHat(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new Sprite(GetPath("blockHelmetPickup"), 0.0f, 0.0f);
            this._sprite = new SpriteMap(GetPath("blockHelmet"), 32, 32, false);
            this._pickupEmptySprite = new Sprite(GetPath("blockEmptyHelmetPickup"), 0.0f, 0.0f);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this._sprite.CenterOrigin();
            this._isArmor = true;
            this._equippedCollisionOffset = new Vec2(-6f, -8f);
            this._equippedCollisionSize = new Vec2(16f, 18f);
            this._hasEquippedCollision = true;
            this._equippedThickness = 3f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._editorName = "Item Block Hat";
            this.editorTooltip = "A nearly indestructible 'helmet'...? Be careful not to get jumped on.";
        }

        public override void Update()
        {
            base.Update();
            if (this._equippedDuck != null && !this.destroyed && !this.spawnedItem)
            {
                this.solid = true;
            }
            else if(this._equippedDuck == null)
            {
                if (!spawnedItem)
                    this.graphic = this._pickupSprite;
                else
                    this.graphic = this._pickupEmptySprite;
            }
        }

        public virtual void SpawnItem()
        {
            PhysicsObject physicsObject = new DuelingPistol(this.center.x, this.center.y);
            int i = Rando.Int(1);
            switch (i)
            {
                case 0:
                    physicsObject = new DuelingPistol(this.center.x, this.center.y);
                    break;
                case 1:
                    physicsObject = new Grenade(this.center.x, this.center.y);
                    break;
                default:
                    physicsObject = new DuelingPistol(this.center.x, this.center.y);
                    break;
            }
            if (physicsObject == null)
                return;
            physicsObject.x = this.x;
            physicsObject.bottom = this.bottom;
            physicsObject.y -= 12f;
            physicsObject.vSpeed = -3.5f;
            physicsObject.clip.Add((MaterialThing)this);
            if (physicsObject is Gun)
            {
                Gun gun = physicsObject as Gun;
                if (gun.CanSpin())
                    gun.angleDegrees = 180f;
            }
            Duck owner = this.owner as Duck;
            physicsObject.clip.Add((MaterialThing)owner);
            owner.clip.Add((MaterialThing)physicsObject);
            Thing.Fondle((Thing)physicsObject, DuckNetwork.localConnection);
            if (!Network.isActive)
            {
                Level.Add((Thing)physicsObject);
                SFX.Play("hitBox", 1f, 0.0f, 0.0f, false);
            }
            else if (this.isServerForObject)
            {
                Level.Add((Thing)physicsObject);
                NetSoundEffect.Play("itemBoxHit");
            }
            spawnedItem = true;
        }

        public override void Equip(Duck d)
        {
            if (this._equippedDuck != null)
                return;
            this.owner = (Thing)d;
            this.solid = true;
            this._equippedDuck = d;
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._equippedDuck != null && !this.destroyed && !this.crushed)
            {
                if (from != ImpactedFrom.Top)
                    return;
                if (with is Duck)
                {
                    if ((double)with.vSpeed > -22.0 * (double)0.2f)
                        with.vSpeed = -22f * 0.2f;
                    with.lastHSpeed = with._hSpeed;
                    with.lastVSpeed = with._vSpeed;
                    (with as Duck).jumping = false;
                    this.DoRumble(with as Duck);
                    if(!spawnedItem)
                        SpawnItem();
                    this.crushed = true;
                }
            }
        }

        protected void DoRumble(Duck duck)
        {
            RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Short, RumbleFalloff.None, RumbleType.Gameplay));
        }

        public override void Crush()
        {
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (this._equippedDuck == null || bullet.owner == this.duck || !bullet.isLocal)
                return false;
            if (this.duck == null)
                return base.Hit(bullet, hitPos);
            Thing.Fondle((Thing)this, DuckNetwork.localConnection);
            //if (bullet.isLocal && Network.isActive)
            //   this._netTing.Play(1f, 0.2f);
            if (Network.isActive)
                NetSoundEffect.Play("equipmentTing");
            else
                SFX.Play("ting2", 1f, 0.0f, 0.0f, false);
            Level.Add((Thing)MetalRebound.New(hitPos.x, hitPos.y, (double)bullet.travelDirNormalized.x > 0.0 ? 1 : -1));
            for (int index = 0; index < 6; ++index)
                Level.Add((Thing)Spark.New(this.x, this.y, bullet.travelDirNormalized, 0.02f));
            return true;
        }
    }
}