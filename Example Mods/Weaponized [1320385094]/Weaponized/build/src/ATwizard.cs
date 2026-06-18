using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class ATwizard : AmmoType
    {
        public float angle;

        public ATwizard()
        {
            this.accuracy = 1f;
            this.range = 300f;
            this.penetration = 2f;
            this.bulletSpeed = 36f;
            this.combustable = true;
            this.ownerSafety = 4;
        }

        public override void PopShell(float x, float y, int dir)
        {
            MagnumShell magnumShell = new MagnumShell(x, y);
            magnumShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)magnumShell);
        }
    }
}
