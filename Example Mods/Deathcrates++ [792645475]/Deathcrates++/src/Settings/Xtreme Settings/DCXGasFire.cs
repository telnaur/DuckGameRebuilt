using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    class DCXGasFire : DeathCrateSetting
    {

        public override void Activate(DeathCrate c, bool server = true)
        {
            float cx = c.x;
            float cy = c.y - 2f;
            Level.Add(new ExplosionPart(cx, cy, true));
            if (server)
            {
                Level.Add(new DeathCrate(c.x, c.y));
                Level.Add(new YellowBarrel(c.x, c.y - 20)
                {
                    vSpeed = -3f
                });
                Grenade g = new Grenade(c.x, c.y);
                g.PressAction();
                g.hSpeed = -1f;
                g.vSpeed = -2f;
                Level.Add(g);
                g = new Grenade(c.x, c.y);
                g.PressAction();
                g.hSpeed = 1f;
                g.vSpeed = -2f;
                Level.Add(g);
                Level.Remove(c);
            }
            Level.Add(new MusketSmoke(c.x, c.y));
        }
    }
}
