using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    class zDC04ColorShift : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            float cx = c.x;
            float cy = c.y - 2f;
            Level.Add(new ExplosionPart(cx, cy, true));


            List<Duck> ducks = Level.CheckCircleAll<Duck>(new Vec2(cx, cy), 2048).ToList();

            ColorShift cs = new ColorShift(ducks);
            Level.Add(cs);

            for (int i = 0; i < ducks.Count; i++)
            {
                Level.Add(new MusketSmoke(ducks[i].position.x, ducks[i].position.y));
            }

            if (server)
            {
                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("explode", 1f, 0f, 0f, false);
        }
    }

    class ColorShift : Thing
    {
        DuckPersona[] persona;
        Vec3[] color;
        public ColorShift(List<Duck> ducks)
        {
            persona = new DuckPersona[ducks.Count()];
            color = new Vec3[ducks.Count()];

            for (int i = 0; i < ducks.Count(); i++)
            {
                //Save personas and original colors
                persona[i] = ducks[i].persona;
                color[i] = ducks[i].persona.color;

                //Recolor to white
                ducks[i].persona.color = new Vec3(255, 255, 255);
                ducks[i].persona.Recreate();
            }

            visible = false;
        }

        public override void Removed()
        {
            for (int i = 0; i < persona.Length; i++)
            {
                persona[i].color = color[i];
                persona[i].Recreate();
            }
            base.Removed();
        }
    }
}

/*
for(int i = 0; i < persona.Length; i++)
{
    persona[i].color = color[i];
    persona[i].Recreate();
}
*/

