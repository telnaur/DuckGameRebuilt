using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class ATShotgunBad : AmmoType
    {
        public ATShotgunBad()
        {
            this.penetration = 1f;
            this.rangeVariation = 10f;
            this.combustable = true;
            this.impactPower = 1f;
            this.range = 100f;
            this.accuracy = 0f;
            this.affectedByGravity = true;
            this.bulletSpeed = 7f;
            this.gravityMultiplier = 1.8f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            ShotgunShell shotgunShell = new ShotgunShell(x, y);
            shotgunShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)shotgunShell);
        }
    }
}