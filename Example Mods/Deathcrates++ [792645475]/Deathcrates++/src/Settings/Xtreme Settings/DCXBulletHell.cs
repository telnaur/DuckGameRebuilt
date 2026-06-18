using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    class DCXBulletHell : DeathCrateSetting
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
                for (int j = 0; j < 18; j++)
                {
                    float dir2 = (float)j * 22.5f + 11.25f;
                    Vec2 move = Maths.AngleToVec(Maths.DegToRad(dir2)) * 2;
                    Vec2 spawn = new Vec2(cx, cy) - move.normalized * 16f;
                    Rando.Float(8f, 14f);
                    QuadLaserBullet ins2 = new QuadLaserBullet(spawn.x, spawn.y, move);
                    Level.Add(ins2);
                }

                for (int j = 0; j < 18; j++)
                {
                    float dir2 = (float)j * 22.5f;
                    Vec2 move = Maths.AngleToVec(Maths.DegToRad(dir2)) * 4;
                    Vec2 spawn = new Vec2(cx, cy) - move.normalized * 16f;
                    Rando.Float(8f, 14f);
                    QuadLaserBullet ins2 = new QuadLaserBullet(spawn.x, spawn.y, move );
                    Level.Add(ins2);
                    //Vec2 idk = new Vec2((float)System.Math.Cos((double)Maths.DegToRad(dir2)), (float)(-(float)System.Math.Sin((double)Maths.DegToRad(dir2))));
                }

                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode", 1f, 0f, 0f, false);
        }
    }
}
