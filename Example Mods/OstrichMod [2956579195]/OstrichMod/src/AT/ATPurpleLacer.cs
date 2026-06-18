using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    public class ATPurpleLacer : AmmoType
    {
        public ATPurpleLacer()
        {
            this.accuracy = 0.2f;
            this.range = 850f;
            this.rangeVariation = 0.0f;
            this.speedVariation = 0.0f;
            this.penetration = 0.4f;
            this.bulletSpeed = 5f;
            this.rebound = true;
            this.sprite = new Sprite(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("PurpleLacer"), 0.0f, 0.0f);
            this.sprite.CenterOrigin();
        }
    }
}
