using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    class zDC02RocketShower : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            float cx = c.x;
            float cy = c.y - 2f;
            Level.Add(new ExplosionPart(cx, cy, true));

            if (server)
            {
                float destination = 0f;
                int dir = Rando.ChooseInt(1, -1);
                ATMissile r = new ATMissile();
                for (int i = 0; i < 10; i++)
                {
                    if (i == 0) { destination = 0; }
                    else { destination = Rando.Float(-128, 128); }
                    r.FireBullet(new Vec2(cx + destination + dir * 640, cy - 640 + i * 32), null, 90 + dir * 45, null);
                }
                Level.Remove(c);
            }
            Graphics.FlashScreen();
			SFX.Play("explode", 1f, 0f, 0f, false);
        }
    }
}
