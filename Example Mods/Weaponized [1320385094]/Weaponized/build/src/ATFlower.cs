using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class ATFlower : AmmoType
    {
        public ATFlower()
        {
            this.accuracy = 0.75f;
            this.range = 250f;
            this.penetration = 1f;
            this.combustable = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            flowerShell flowerShell = new flowerShell(x, y);
            flowerShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)flowerShell);
        }
    }
}
