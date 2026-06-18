using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class DCWizardSpell : DeathCrateSetting
    {
        private int ammo = 1;
        public Duck chosenDuck;
        public List<Duck> ducksInRange = new List<Duck>();
        public override void Activate(DeathCrate c, bool server = true)
        {
            if (this.ammo > 0)
            {
                ducksInRange.Clear();
                foreach (Duck duck in Level.CheckCircleAll<Duck>(new Vec2(c.x, c.y), 10000f))
                {
                    if (!duck.dead)
                    {
                        ducksInRange.Add(duck);
                    }
                }
                if (ducksInRange.Any())
                {
                    var rnd = new Random();
                    ducksInRange = ducksInRange.OrderBy(item => rnd.Next()).ToList();
                    chosenDuck = ducksInRange.ElementAt(0);
                    Level.Add((Thing)new wizardSound());
                    for (int j = 0; j < 3; j++)
                    {
                        Level.Add(SmallSmoke.New(c.x + Rando.Float(-0.5f, 0.5f), c.y - 16f + Rando.Float(-0.5f, 0.5f)));
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        Level.Add(SmallSmoke.New(chosenDuck.x + Rando.Float(-5f, 5f), chosenDuck.y + Rando.Float(-5f, 5f)));
                    }
                    Level.Add((Thing)new blueScreen(chosenDuck.x, chosenDuck.y, true));
                    chosenDuck.y = 10000f;
                    Level.Remove((Thing)c);
                    Graphics.FlashScreen();
                    //SFX.Play("explode", 1f, 0.0f, 0.0f, false);
                    this.ammo--;
                    //chosenDuck.Fondle((Thing)this);
                }
            }
        }
    }
}