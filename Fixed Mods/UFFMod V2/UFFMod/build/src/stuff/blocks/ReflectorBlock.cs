using System.Collections.Generic;
using System.Linq;
using System;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|stuff|blocks")]
    public class ReflectorBlock : Block, IDontMove, IPathNodeBlocker, IPlatform
    {
        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding netSFX_metingStateBinding = new NetSoundBinding("netSFX_meting");

        public NetSoundEffect netSFX_meting = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\meting")
        })
        {
            volume = 0.2f
        };

        private SpriteMap sprite;
        private int shimmerCountdown;

        public ReflectorBlock(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _editorName = "Reflector Block";
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\blocks\\reflectorBlock"), 16, 16);
            sprite.AddAnimation("normal", 1f, false, 0);
            sprite.AddAnimation("shimmer", 0.25f, false, 1, 2, 3, 4, 5);
            sprite.SetAnimation("normal");
            graphic = sprite;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            depth = 0.5f;
            shimmerCountdown = 20 + Rando.Int(60);
        }

        public override void Update()
        {
            if (shimmerCountdown == 0)
            {
                sprite.SetAnimation("shimmer");
                shimmerCountdown = 150 + Rando.Int(60);
            }
            else
                shimmerCountdown--;

            if (sprite.currentAnimation.Equals("shimmer") && sprite.finished)
                sprite.SetAnimation("normal");

            base.Update();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.owner == this)
                return false;

            if (!bullet.rebound)
            {
                float angleAdjust = 0f;
                float xHit = position.x - hitPos.x;
                float yHit = position.y - hitPos.y;
                bool isXHit = Math.Abs(xHit) > Math.Abs(yHit);
                float xAdjust = xHit > 0 ? -1f : 1f;
                float yAdjust = yHit > 0 ? -1f : 1f;
                bool xBlocked = Level.CheckPoint<Block>(hitPos.x + xAdjust, hitPos.y) != null;
                bool yBlocked = Level.CheckPoint<Block>(hitPos.x, hitPos.y + yAdjust) != null;
                if (isXHit && xBlocked)
                    isXHit = false;
                if (!isXHit && yBlocked)
                    isXHit = true;
                if (xBlocked && yBlocked)
                    return true;
                if (isXHit)
                    angleAdjust = 180f;
                Bullet reboundBullet = bullet.ammo.GetBullet(hitPos.x, hitPos.y, this, bullet.angle + angleAdjust, bullet.firedFrom, bullet.range - ((bullet.bulletDistance > 0f) ? bullet.bulletDistance : bullet.range / 2f));
                Level.Add(reboundBullet);
                Level.Add(new SerpentRebound(hitPos.x, hitPos.y));
                if (bullet.isLocal && Network.isActive)
                    netSFX_meting.Play();
                else
                    SFX.Play(Mod.GetPath<UffMod>("SFX\\meting"), 0.2f);
            }
            return true;
        }
    }
}