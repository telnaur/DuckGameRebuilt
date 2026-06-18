using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    internal class ATForce : AmmoType
    {
        public ATForce()
        {
            this.accuracy = 1f;
            this.range = 1000f;
            this.bulletSpeed = 0.04f;
            this.rangeVariation = 0.0f;
            this.speedVariation = 0.0f;
            this.bulletThickness = 3f;
            this.penetration = 90f;
            this.affectedByGravity = false;
            this.rebound = false;
	          this.bulletLength = 1f;
            this.sprite = new Sprite(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("ATForce"), 0.0f, 0.0f);
            this.sprite.CenterOrigin();
        }

        public override void OnHit(bool destroyed, Bullet b)
        {
            if ((double)this.penetration >= 2.0)
            {
                foreach (Door door in Level.CheckCircleAll<Door>(b.position, 2f))
                {
                    if (b.isLocal)
                        Thing.Fondle((Thing)door, DuckNetwork.localConnection);
                    if (Level.CheckLine<Block>(b.position, door.position, (Thing)door) == null)
                        door.Destroy((DestroyType)new DTImpact((Thing)b));
                }
            }
            base.OnHit(destroyed, b);
        }
    }
}


