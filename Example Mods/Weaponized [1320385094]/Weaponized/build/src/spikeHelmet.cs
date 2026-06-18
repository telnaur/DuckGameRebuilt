using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    [EditorGroup("Zyrafa|Equipment|Offline")]
    [BaggedProperty("isOnlineCapable", false)]
    public class spikeHelmet : Helmet
    {

        private float sparkTimer = 0.8f;
        protected SpriteMap _electricitySprite;
        public bool teleported = false;
        public Duck teleportationOwner;
        public Vec2 attackerPosFirst;
        public Vec2 hitPosFirst;



        public spikeHelmet(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new Sprite(GetPath("spikeHelmetPickup"), 0.0f, 0.0f);
            this._sprite = new SpriteMap(GetPath("spikeHelmet"), 32, 32, false);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-5f, -2f);
            this.collisionSize = new Vec2(12f, 8f);
            this._sprite.CenterOrigin();
            this._isArmor = true;
            this._equippedThickness = 3f;
            this._electricitySprite = new SpriteMap(GetPath("spikeHelmetElectricity"), 32, 32, false);
            this._electricitySprite.center = new Vec2(16f, 16f);
            this._electricitySprite.AddAnimation("worn", 0.5f, true, 0, 1, 2, 3);
            this._electricitySprite.SetAnimation("worn");
            this._editorName = "Teleporter Helmet";
            this.editorTooltip = "When hit, swaps the shooter's and the wearer's positions, so quite mind-boggling.";
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (this._equippedDuck == null || bullet.owner == this.duck || !bullet.isLocal)
                return false;
            if (bullet.owner != null && this.crushed == false)
            {
                teleportationOwner = this._equippedDuck;
                hitPosFirst = new Vec2(teleportationOwner.position.x, teleportationOwner.position.y);
                attackerPosFirst = new Vec2(bullet.owner.position.x, bullet.owner.position.y);
                teleportationOwner.position = attackerPosFirst;
                if (bullet.owner is Duck)
                {
                    bullet.owner.position = hitPosFirst;
                }
                for (int j = 0; j < 8; j++)
                {
                    Level.Add(SmallSmoke.New(attackerPosFirst.x + Rando.Float(-5f, 5f), attackerPosFirst.y + Rando.Float(-8f, 8f)));
                    Level.Add(SmallSmoke.New(hitPosFirst.x + Rando.Float(-5f, 5f), hitPosFirst.y + Rando.Float(-8f, 8f)));
                }
                Graphics.FlashScreen();
                /*
                if(bullet.owner is Duck)
                {
                    Duck bulletOwner = bullet.owner as Duck;
                    bulletOwner.hSpeed = 0f;
                    bulletOwner.GoRagdoll();
                    bulletOwner.crippleTimer = 1.5f;
                }*/
                bullet.owner.hSpeed = 0f;
                teleportationOwner.hSpeed = 0f;
                teleported = true;
            }
            if (bullet.isLocal)
            {
                this.duck.KnockOffEquipment(this, true, bullet);
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
            }
            if (bullet.isLocal && Network.isActive)
                NetSoundEffect.Play("equipmentTing");
            if (bullet.isLocal && Network.isActive)
                NetSoundEffect.Play("duckSwear"); 
            bullet.hitArmor = true;
            Level.Add((Thing)MetalRebound.New(hitPos.x, hitPos.y, (double)bullet.travelDirNormalized.x > 0.0 ? 1 : -1));
            for (int index = 0; index < 6; ++index)
                Level.Add((Thing)Spark.New(this.x, this.y, bullet.travelDirNormalized, 0.02f));

            return true;
        }

        public override void Update()
        {
            base.Update();
            if (this.crushed)
            {
                this.sparkTimer -= 0.01f;
                if (sparkTimer < 0f)
                {
                    for (int index = 0; index < 6; ++index)
                        Level.Add((Thing)Spark.New(this.x, this.y - 2f, new Vec2(Rando.Float(-1f, 1f), Rando.Float(0.3f, 0.7f)), 0.03f));
                    sparkTimer = Rando.Float(0.4f, 1f);
                }
            }
            /*
            if (this.teleported)
            {
                teleportationOwner.position = attackerPosFirst;
                teleported = false;
            }
            if (this._equippedDuck != null)
            {
                teleportationOwner = this._equippedDuck;
            }*/
        }

        public override void Draw()
        {
            base.Draw();
            Vec2 vec2 = new Vec2(16f, 16f);
            if (this._equippedDuck != null && !this.crushed)
            {
                this.Draw((Sprite)this._electricitySprite, new Vec2(vec2.x - 16f, vec2.y - 16f), 1);
            }
        }
    }
}