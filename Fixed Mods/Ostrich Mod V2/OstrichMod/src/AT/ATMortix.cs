using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    internal class ATMortix : AmmoType
    {
        public ATMortix()
        {
            this.accuracy = 1f;
            this.range = 470f;
            this.bulletSpeed = 5f;
            this.combustable = false;
            this.penetration = 10f;
            this.affectedByGravity = true;
	    this.barrelAngleDegrees = -90f;
	    this.bulletLength = 2f;
            this.sprite = new Sprite(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("ATMortix"), 0.0f, 0.0f);
            this.sprite.CenterOrigin();
         }

    }
}
