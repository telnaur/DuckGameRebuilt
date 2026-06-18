using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    class DCXRomanCandle : DeathCrateSetting
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
            if (server)
            {
                for (int j = 0; j < 3; j++)
                {
                    Thing k;
                    if (j == 1)
                    {
                        k = new FireExtinguisher(c.x, c.y);
                    }
                    else
                    {
                        k = new RomanCandle(c.x, c.y);
                        (k as Gun).infinite = true;
                        (k as Gun).PressAction();
                    }
                    float norm = (float)j / 2f;
                    k.hSpeed = (-15f + norm * 30f) * Rando.Float(0.5f, 1f);
                    k.vSpeed = Rando.Float(-3f, -11f);
                    Level.Add(k);
                }
                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode", 1f, 0f, 0f, false);
        }
    }
}
