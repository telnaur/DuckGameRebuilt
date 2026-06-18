using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    class DCXNetCity : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            float cx = c.x;
            float cy = c.y - 2f;
            Level.Add(new ExplosionPart(cx, cy, true));
            int num = 6;
            if (Graphics.effectsLevel < 2)
            {
                num = 3;
            }
            for (int i = 0; i < num; i++)
            {
                float dir = (float)i * 60f + Rando.Float(-10f, 10f);
                float dist = Rando.Float(12f, 20f);
                ExplosionPart ins = new ExplosionPart(cx + (float)(System.Math.Cos((double)Maths.DegToRad(dir)) * (double)dist), cy - (float)(System.Math.Sin((double)Maths.DegToRad(dir)) * (double)dist), true);
                Level.Add(ins);
            }

            IEnumerable<Duck> allDucks = Level.CheckCircleAll<Duck>(new Vec2(cx, cy), 2048);
            foreach(Duck duck in allDucks)
            {
                duck.Netted(new Net(duck.position.x, duck.position.y, duck));
            }

            if (server)
            {
                for (int j = 0; j < 8; j++)
                {
                    float dir2 = (float)j * 22.5f + Rando.Float(-8f, 8f);
                    float dist2 = Rando.Float(8f, 14f);
                    Level.Add(new NetGun(c.x, c.y)
                    {
                        hSpeed = (float)System.Math.Cos((double)Maths.DegToRad(dir2)) * dist2,
                        vSpeed = (float)(-(float)System.Math.Sin((double)Maths.DegToRad(dir2))) * dist2
                    });
                }
                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode", 1f, 0f, 0f, false);
        }
    }
}
