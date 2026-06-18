using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class ATShrekShrapnel : AmmoType
    {
        public ATShrekShrapnel()
        {
            this.accuracy = 0.75f;
            this.range = 250f;
            this.penetration = 1f;
            this.bulletSpeed = 18f;
            this.combustable = true;
            this.bulletColor = Color.DarkOliveGreen;
            this.impactPower = 12f;
        }

        public override void MakeNetEffect(Vec2 pos, bool fromNetwork = false)
        {
            Level.Add((Thing)new ExplosionPart(pos.x + Rando.Float(-2f, 2f), pos.y + Rando.Float(-2f, 2f), false));
            for (int index = 0; index < 4; ++index)
                Level.Add((Thing)new ExplosionPart(pos.x + Rando.Float(-11f, 11f), pos.y + Rando.Float(-11f, 11f), false));
            if (fromNetwork)
            {
                foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(pos, 70f))
                {
                    if (physicsObject.isServerForObject)
                    {
                        physicsObject.sleeping = false;
                        physicsObject.vSpeed = -2f;
                    }
                }
            }
            SFX.Play("explode", 1f, 0.0f, 0.0f, false);
        }
    }
}
