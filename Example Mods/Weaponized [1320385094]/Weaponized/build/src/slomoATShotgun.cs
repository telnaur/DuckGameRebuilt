using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class slomoATShotgun : AmmoType
    {
        public slomoATShotgun()
        {
            this.accuracy = 0.6f;
            this.range = 220f;
            this.penetration = 0.4f;
            this.rangeVariation = 10f;
            this.combustable = true;
            this.bulletSpeed = 1.1f;
            this.speedVariation = 0.2f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            slomoShotgunShell slomoShotgunShell = new slomoShotgunShell(x, y);
            slomoShotgunShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)slomoShotgunShell);
        }
    }
}
