using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class slomoATShrapnel : AmmoType
    {
        public slomoATShrapnel()
        {
            this.accuracy = 0.75f;
            this.range = 300f;
            this.penetration = 0.4f;
            this.bulletSpeed = 0.8f;
            this.speedVariation = 0.2f;
            this.combustable = true;
        }
    }
}
