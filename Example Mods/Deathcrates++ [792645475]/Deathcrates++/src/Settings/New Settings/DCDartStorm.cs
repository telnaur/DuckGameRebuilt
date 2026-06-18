using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    class zDC03DartStorm : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            float cx = c.x;
            float cy = c.y - 2f;
            Level.Add(new ExplosionPart(cx, cy, true));

            if (server)
            {
                for (int i = 0; i < 60; i++)
                {
                    float fireAngle = i * 6f;
                    Vec2 travelDir = Maths.AngleToVec(fireAngle);
                    Dart d = new Dart(cx + travelDir.x * 50, cy + travelDir.y * 50, null as Duck, -fireAngle);
                    d.Fondle(d);
                    if (i % 6 == 0)
                    {
                        Level.Add(SmallFire.New(0f, 0f, 0f, 0f, false, d, true, null, false));
                        d.burning = true;
                        d.onFire = true;
                    }
                    d.hSpeed = travelDir.x * 10f;
                    d.vSpeed = travelDir.y * 10f;
                    Level.Add(d);
                }
                Chaindart cd = new Chaindart(cx, cy);
                Level.Add(cd);
                foreach(Duck duck in Level.CheckCircleAll<Duck>(new Vec2(cx, cy), 2048))
                {
                    duck.Disarm(cd);
                }
                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode", 1f, 0f, 0f, false);
        }
    }
}
