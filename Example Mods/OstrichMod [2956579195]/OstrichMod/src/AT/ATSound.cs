using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace DuckGame.OstrichMod
{
    internal class ATSound : AmmoType
    {
        public ATSound()
        {
            this.accuracy = 1f;
            this.range = 800f;
            this.bulletSpeed = 9.5f;
            this.combustable = false;
            this.penetration = 0f;
            this.affectedByGravity = false;
            this.rebound = true;
	    this.bulletLength = 1f;
            this.sprite = new Sprite(Mod.GetPath<DuckGame.OstrichMod.OstrichMod>("Sound"), 0.0f, 0.0f);
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


