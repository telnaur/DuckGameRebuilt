using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class DCNailArmaggedon : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            float x = c.x;
            float ypos = c.y - 2f;
            Level.Add((Thing)new ExplosionPart(x, ypos, true));
            int num1 = 6;
            if (Graphics.effectsLevel < 2)
                num1 = 3;
            for (int index = 0; index < num1; ++index)
            {
                float deg = (float)index * 60f + Rando.Float(-10f, 10f);
                float num2 = Rando.Float(12f, 20f);
                Level.Add((Thing)new ExplosionPart(x + (float)Math.Cos((double)Maths.DegToRad(deg)) * num2, ypos - (float)Math.Sin((double)Maths.DegToRad(deg)) * num2, true));
            }
            if (server)
            {
                for (int index = 0; index < 30; ++index)
                {
                    float num2 = (float)(90) + Rando.Float(-40f, 40f);
                    ATnail atNail = new ATnail();
                    atNail.bulletSpeed = 7f;
                    Bullet bullet = new Bullet(x + (float)(Math.Cos((double)Maths.DegToRad(num2)) * 6.0), ypos - 15f, (AmmoType)atNail, num2, (Thing)null, false, -1f, false, true);
                    Level.Add((Thing)bullet);
                }
                Level.Remove((Thing)c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode", 1f, 0.0f, 0.0f, false);
        }
    }
}