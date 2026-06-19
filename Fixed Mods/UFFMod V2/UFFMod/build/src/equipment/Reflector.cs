using System;
using System.Linq;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|equipment|barrier")]
    public class Reflector : Barrier, IQuackOverrideEquipment
    {
        public StateBinding _cooldownStateBinding = new StateBinding("_cooldown");
        public StateBinding netSFX_metingStateBinding = new NetSoundBinding("netSFX_meting");

        public NetSoundEffect netSFX_meting = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\meting")
        })
        {
            volume = 0.2f
        };

        public int _cooldown;

        public int cooldown
        {
            get
            {
                return _cooldown;
            }
        }

        public Reflector(float xpos, float ypos)
            : base(xpos, ypos)
        {
            // editor settings
            _editorName = "Reflector";
            
            // collision & sprite settings
            pickupSprite = new Sprite(Mod.GetPath<UffMod>("equipment\\reflectorPickup"));
            sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\reflector"), 32, 32);
            sprite.AddAnimation("active", 0.75f, true, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            sprite.AddAnimation("reflection", 1f, false, 12, 12, 12);
            sprite.SetAnimation("active");
            graphic = pickupSprite;
            center = new Vec2(6f, 4f);
            _holdOffset = new Vec2(1f, 1f);
            wearCenter = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-3f, -3f);
            collisionSize = new Vec2(6f, 6f);
            _equippedCollisionOffset = new Vec2(-15f, -15f);
            _equippedCollisionSize = new Vec2(30f, 30f);
            _hasEquippedCollision = true;
            sprite.CenterOrigin();

            // equipment settings
            _equippedThickness = 0f;
            flammable = 0f;
        }

        public override void Update()
        {
            /*
            if (_trueHitPoints <= 0f && equippedDuck != null)
            {
                equippedDuck.KnockOffEquipment(this);
                Thing.Fondle(this, DuckNetwork.localConnection);
                if (isServerForObject)
                    _netTing.Play();
            }
            */

            base.Update();

            if (equippedDuck != null
                && equippedDuck.inputProfile.Pressed(Triggers.Quack)
                && _cooldown == 0
                &&
                !duck.immobilized
                && !duck.inNet
                && !duck.sliding)
                _cooldown = 240;

            if (sprite.finished && sprite.currentAnimation.Equals("reflection"))
                sprite.SetAnimation("active");

            if (_cooldown > 0)
                _cooldown--;
            if (equippedDuck == null || sprite.currentAnimation.Equals("reflection") || (_cooldown > 180 && _cooldown <= 240))
                alpha = 1f;
            else if (equippedDuck != null)
            {
                if (_cooldown == 0)
                    alpha = 0.25f;
                else if (_cooldown <= 180)
                    alpha = 0f;
            }
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (equippedDuck == null)
                return base.Hit(bullet, hitPos);

            if (bullet.owner == equippedDuck || _cooldown <= 180)
                return false;

            Reflect(bullet, hitPos);
            return true;
        }

        public void Reflect(Bullet bullet, Vec2 hitPos)
        {
            // _trueHitPoints -= bullet.ammo.penetration;
            float angleAdjust = 0f;
            float xHit = position.x - hitPos.x;
            float yHit = position.y - hitPos.y;
            if (Math.Abs(xHit) > Math.Abs(yHit))
                angleAdjust = 180f;
            Bullet reboundBullet = bullet.ammo.GetBullet(hitPos.x, hitPos.y, equippedDuck, bullet.angle + angleAdjust, bullet.firedFrom, bullet.range);
            Level.Add(reboundBullet);
            Level.Add(new SerpentRebound(hitPos.x, hitPos.y));
            if (bullet.isLocal && Network.isActive)
                netSFX_meting.Play();
            else
                SFX.Play(Mod.GetPath<UffMod>("SFX\\meting"), 0.2f);
            sprite.SetAnimation("reflection");
            /*
            if (_trueHitPoints <= 0f)
            {
                equippedDuck.KnockOffEquipment(this, true, bullet.travelDirNormalized);
                Thing.Fondle(this, DuckNetwork.localConnection);
                if (bullet.isLocal && Network.isActive)
                    _netTing.Play();
            }
            */
        }
    }
}
