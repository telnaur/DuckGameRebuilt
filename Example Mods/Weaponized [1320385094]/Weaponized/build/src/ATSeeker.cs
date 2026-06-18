using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class ATSeeker : AmmoType
    {

        public ATSeeker()
        {
            this.accuracy = 0.5f;
            this.range = 320f;
            this.penetration = 1f;
            this.bulletSpeed = 5f;
            this.rebound = true;
            this.bulletThickness = 0.3f;
            this.bulletType = typeof(LaserBullet);
            this.bulletLength = 40f;
        }
    }
}