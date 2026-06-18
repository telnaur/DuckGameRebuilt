using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    class DCXSwordAdvenure : DeathCrateSetting
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
                for (int j = 0; j < 8; j++)
                {
                    Sword p = new Sword(c.x, c.y);
                    float norm = (float)j / 7f;
                    p.hSpeed = (-15f + norm * 30f) * Rando.Float(0.5f, 1f);
                    p.vSpeed = Rando.Float(-10f, 10f);
                    p._wasLifted = true;
                    p._framesExisting = 16;
                    Level.Add(p);
                }

                for (int i = 0; i < 4; i++)
                {
                    SledgeHammer sh = new SledgeHammer(cx, cy);
                    float norm = (float)i / 7f;
                    sh.hSpeed = (-15f + norm * 30f) * Rando.Float(0.6f, 1.2f);
                    sh.vSpeed = Rando.Float(-15f, 5f);
                    Level.Add(sh);
                }
                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode", 1f, 0f, 0f, false);
        }
    }
}
