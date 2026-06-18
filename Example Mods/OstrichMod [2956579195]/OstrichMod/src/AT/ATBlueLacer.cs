using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    internal class ATBlueLacer : AmmoType
    {
        public ATBlueLacer()
        {
            this.accuracy = 1f;
            this.range = 800f;
            this.bulletSpeed = 8f;
            this.combustable = false;
            this.penetration = 0.1f;
            this.affectedByGravity = true;
            this.rebound = false;
            this.bulletLength = 3f;
            this.sprite = new Sprite(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("BlueLacer"), 0.0f, 0.0f);
            this.sprite.CenterOrigin();
        }
        public override void OnHit(bool destroyed, Bullet b)
        {
            if (!b.isLocal)
            {
                return;
            }
            Level.Add(new IceCloud(b.x, b.y, 1f));
            base.OnHit(destroyed, b);
        }
    }
}

