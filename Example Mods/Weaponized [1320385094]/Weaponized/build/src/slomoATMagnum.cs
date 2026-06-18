using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class slomoATMagnum : AmmoType
    {
        public float angle;

        public slomoATMagnum()
        {
            this.accuracy = 1f;
            this.range = 360f;
            this.penetration = 2f;
            this.bulletSpeed = 36f;
            this.combustable = true;
            this.bulletSpeed = 2f;
            this.speedVariation = 0.2f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            slomoMagnumShell slomoMagnumShell = new slomoMagnumShell(x, y);
            slomoMagnumShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)slomoMagnumShell);
        }
    }
}
