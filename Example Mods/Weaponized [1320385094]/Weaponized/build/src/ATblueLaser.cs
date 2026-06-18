using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class ATblueLaser : AmmoType
    {
        public bool angleShot = true;
        public Depth depth { get; set; }
        public ATblueLaser()
        {
            this.accuracy = 0.85f;
            this.range = 140f;
            this.penetration = 1f;
            this.bulletSpeed = 18f;
            this.bulletThickness = 0.4f;
            this.bulletType = typeof(blueLaserBullet);
            this.depth = (Depth)1.5f;
        }
    }
}
