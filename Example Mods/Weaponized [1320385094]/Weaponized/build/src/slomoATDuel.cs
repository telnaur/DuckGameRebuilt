using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class slomoATDuel : AmmoType
    {
        public slomoATDuel()
        {
            this.accuracy = 0.8f;
            this.range = 5000f;
            this.penetration = 0.4f;
            this.bulletSpeed = 1.3f;
            this.speedVariation = 0.2f;
            this.combustable = true;
            this.rebound = false;
        }
    }
}
