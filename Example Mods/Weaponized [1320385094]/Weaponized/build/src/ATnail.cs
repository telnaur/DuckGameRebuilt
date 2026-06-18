using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class ATnail : AmmoType
    {

        public ATnail()
        {
            this.accuracy = 0.95f;
            this.penetration = 0.4f;
            this.bulletSpeed = 15f;
            this.rangeVariation = 0.0f;
            this.speedVariation = 0.0f;
            this.range = 3000f;
            this.rebound = false;
            this.affectedByGravity = true;
            this.weight = 2f;
            this.immediatelyDeadly = true;
            this.bulletThickness = 1f;
            this.bulletColor = Color.White;
            this.sprite = new Sprite(Mod.GetPath<DuckGame.MyMod.MyMod>("nailAmmo"), 0.0f, 0.0f);
            this.sprite.CenterOrigin();
        }
    }
}
