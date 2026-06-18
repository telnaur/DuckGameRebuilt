using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class slomoATSniper : AmmoType
    {
        public slomoATSniper()
        {
            this.combustable = true;
            this.range = 1500f;
            this.accuracy = 1f;
            this.penetration = 2f;
            this.bulletSpeed = 2.5f;
            this.speedVariation = 0.2f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            slomoSniperShell sniperShell = new slomoSniperShell(x, y);
            sniperShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)sniperShell);
        }
    }
}
