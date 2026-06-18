using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DuckGame;
using DuckGame.TarGunMod;

namespace TarGun.src
{
    // most of this is recycled code of DCNetCity
    public class DeathCrateEvent : DeathCrateSetting
    {
        public override void Activate(DeathCrate crate, bool server = true)
        {
            float xpos = crate.x;
            float ypos = crate.y - 2f;
            Level.Add((Thing)new ExplosionPart(xpos, ypos));
            int num1 = 6;
            if (Graphics.effectsLevel < 2)
                num1 = 3;
            for (int index = 0; index < num1; ++index)
            {
                float deg = (float)index * 60f + Rando.Float(-10f, 10f);
                float num2 = Rando.Float(12f, 20f);
                Level.Add((Thing)new ExplosionPart(xpos + (float)Math.Cos((double)Maths.DegToRad(deg)) * num2, ypos - (float)Math.Sin((double)Maths.DegToRad(deg)) * num2));
            }
            if (server)
            {
                for (int i = 0; i < 16; i++)
                {
                    float deg = i * 22.5f + Rando.Float(-8f, 8f);
                    TarBlast projectile = new TarBlast(xpos, ypos, Rando.Float(0.875f, 1.25f), 4);
                    projectile.hSpeed = (float)Math.Cos(Maths.DegToRad(deg)) * (Rando.Float(4f, 7f));
                    projectile.vSpeed = (float)-Math.Sin(Maths.DegToRad(deg)) * (Rando.Float(4f, 7f));
                    if (deg > 0 && deg < 180) projectile.vSpeed -= (float)Math.Sqrt(Math.Abs(90f - deg)) / 5;
                    Level.Add(projectile);
                }
                for (int i = 0; i < 16; ++i)
                {
                    float deg = i * 11.25f + Rando.Float(-8f, 8f);
                    Vec2 projDir = Maths.AngleToVec(deg + Rando.Float(-0.2f, 0.2f)) * new Vec2(Rando.Float(2f, 6f), Rando.Float(2f, 4f));
                    Level.Add(TarParticle.New(xpos - crate.hSpeed, ypos - crate.vSpeed, projDir.x, projDir.y, 1.25f));
                }
                Level.Remove(crate);
            }
            SFX.Play("corkFire");
            SFX.Play("campingEmpty");
            RumbleManager.AddRumbleEvent(crate.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
        }
    }
}
