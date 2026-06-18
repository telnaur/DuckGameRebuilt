using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class tinyATShrapnel : AmmoType
    {
        public tinyATShrapnel()
        {
            this.accuracy = -2f;
            this.range = 55f;
            this.rangeVariation = 15f;
            this.penetration = 1f;
            this.bulletSpeed = 18f;
            this.combustable = true;
        }
    }
}
