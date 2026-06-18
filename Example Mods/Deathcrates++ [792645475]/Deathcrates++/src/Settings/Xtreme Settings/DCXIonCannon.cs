using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    class DCXIonCannon : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            float cx = c.x;
            float cy = c.y - 2f;
            Level.Add(new ExplosionPart(cx, cy, true));
            Level.Add(new IonCannon(new Vec2(c.x + 3000f, c.y + 3000f), new Vec2(c.x - 3000f, c.y - 3000f))
            {
                serverVersion = server
            });
            Level.Add(new IonCannon(new Vec2(c.x - 3000f, c.y + 3000f), new Vec2(c.x + 3000f, c.y - 3000f))
            {
                serverVersion = server
            });
            Graphics.FlashScreen();
            SFX.Play("laserBlast", 1f, 0f, 0f, false);
            if (server)
            {
                Level.Remove(c);
            }
        }
    }
}
