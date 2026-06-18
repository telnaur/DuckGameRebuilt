using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    internal class ATToxic : AmmoType
    {
        public ATToxic()
        {
            this.accuracy = 1f;
            this.range = 800f;
            this.bulletSpeed = 8f;
            this.combustable = false;
            this.penetration = 0.4f;
            this.affectedByGravity = false;
            this.rebound = false;
            this.bulletColor = Color.DarkGreen;
        }

        public override void OnHit(bool destroyed, Bullet b)
        {
            if (!b.isLocal)
                return;

            ToxicSmoke toxic_smoke = new ToxicSmoke(b.position.x, b.position.y, 9f + Rando.Float(1f));
            toxic_smoke.inflamable = false;
            Level.Add(toxic_smoke);
            base.OnHit(destroyed, b);
        }
    }
}

