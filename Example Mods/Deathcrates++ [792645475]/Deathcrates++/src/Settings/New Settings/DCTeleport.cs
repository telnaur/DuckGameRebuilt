using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    class zDC01Teleport : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            if (!Network.isActive)
            {
                int swapAmount;
                float cx = c.x;
                float cy = c.y - 2f;
                Level.Add(new ExplosionPart(cx, cy, true));
                Level.Add(new MusketSmoke(c.x, c.y));
                if (server)
                {
                    //List<Thing> isRagdoll = new List<Thing>();
                    IEnumerable<Ragdoll> ragdolledDucks = Level.CheckCircleAll<Ragdoll>(new Vec2(cx, cy), 2048);
                    foreach (Ragdoll duck in ragdolledDucks)
                    {
                        duck.Unragdoll();
                        //isRagdoll.Add(duck);
                    }

                    List<Duck> allDucks = Level.CheckCircleAll<Duck>(new Vec2(cx, cy), 2048).ToList<Duck>();
                    List<Vec2> duckPositions = new List<Vec2>();

                    foreach (Duck duck in allDucks)
                    {
                        Level.Add(new MusketSmoke(duck.position.x, duck.position.y));
                        duckPositions.Add(duck.position);
                        duck.visible = false;
                    }

                    switch (allDucks.Count)
                    {
                        case 1:
                            break;
                        case 2:
                            allDucks[0].position = duckPositions[1];
                            allDucks[1].position = duckPositions[0];
                            break;
                        case 3:
                            swapAmount = Rando.ChooseInt(1, 2);
                            if (swapAmount == 1)
                            {
                                allDucks[0].position = duckPositions[1];
                                allDucks[1].position = duckPositions[2];
                                allDucks[2].position = duckPositions[0];
                            }
                            else
                            {
                                allDucks[0].position = duckPositions[2];
                                allDucks[1].position = duckPositions[0];
                                allDucks[2].position = duckPositions[1];
                            }
                            break;
                        case 4:
                            swapAmount = Rando.ChooseInt(1, 2, 3);
                            switch (swapAmount)
                            {
                                case 1:
                                    allDucks[0].position = duckPositions[1];
                                    allDucks[1].position = duckPositions[2];
                                    allDucks[2].position = duckPositions[3];
                                    allDucks[3].position = duckPositions[0];
                                    break;
                                case 2:
                                    allDucks[0].position = duckPositions[2];
                                    allDucks[1].position = duckPositions[3];
                                    allDucks[2].position = duckPositions[0];
                                    allDucks[3].position = duckPositions[1];
                                    break;
                                case 3:
                                    allDucks[0].position = duckPositions[3];
                                    allDucks[1].position = duckPositions[0];
                                    allDucks[2].position = duckPositions[1];
                                    allDucks[3].position = duckPositions[2];
                                    break;
                            }
                            break;
                    }
                    foreach (Duck duck in allDucks)
                    {
                        duck.vSpeed += 0.001f;
                        duck.visible = true;
                    }

                    /*foreach (Duck duck in isRagdoll)
                    {
                        duck.GoRagdoll();
                    }*/
                    Level.Remove(c);
                }
                Graphics.FlashScreen();
                SFX.Play("explode", 1f, 0f, 0f, false);
            }
            else
            {
                if (server)
                {
                    SFX.Play("quack", 2, 0, 0, false);

                    Level.Add(new MusketSmoke(c.x, c.y));
                    Level.Remove(c);
                }
                Graphics.FlashScreen();
                SFX.Play("explode", 1f, 0f, 0f, false);
            }
        }
    }
}