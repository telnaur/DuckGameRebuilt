using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeathcratesPlusPlus
{
    class zDC00Earthquake : DeathCrateSetting
    {
        public override void Activate(DeathCrate c, bool server = true)
        {
            float cx = c.x;
            float cy = c.y - 2f;
            Level.Add(new ExplosionPart(cx, cy, true));
            
            if (server)
            {
                using (IEnumerator<Thing> enumerator = Level.current.things.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        PhysicsObject physicsObject = enumerator.Current as PhysicsObject;
                        if (physicsObject != null && physicsObject.active && physicsObject.visible && physicsObject.grounded && (!(physicsObject is Holdable) || ((Holdable)physicsObject).duck == null))
                        {
                            if (physicsObject.isServerForObject)
                            {
                                physicsObject.hSpeed = Rando.Float(-5f, 5f);
                                physicsObject.vSpeed = -3f;
                                Duck duck = physicsObject as Duck;
                                if (duck != null)
                                {
                                    Holdable holdObject = duck.holdObject;
                                    if (holdObject != null)
                                    {
                                        duck.ThrowItem(false);
                                        physicsObject.vSpeed -= 4f;
                                        physicsObject.hSpeed = duck.hSpeed * 0.8f;
                                        physicsObject.clip.Add(duck);
                                        duck.clip.Add(holdObject);
                                    }
                                    duck.GoRagdoll();
                                    if (holdObject != null)
                                    {
                                        duck.ragdoll.part1.clip.Add(holdObject);
                                        duck.ragdoll.part2.clip.Add(holdObject);
                                        duck.ragdoll.part3.clip.Add(holdObject);
                                        holdObject.clip.Add(duck.ragdoll.part1);
                                        holdObject.clip.Add(duck.ragdoll.part2);
                                        holdObject.clip.Add(duck.ragdoll.part3);
                                    }
                                }
                                Gun gun = physicsObject as Gun;
                                if (gun != null)
                                {
                                    gun.PressAction();
                                }
                            }
                        }
                    }
                }

                Level.Remove(c);
            }
            Graphics.FlashScreen();
            SFX.Play("rockHitGround2", 1f, 0f, 0f, false);
        }
    }

}

/*POW:

    using (IEnumerator<Thing> enumerator = Level.current.things.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PhysicsObject physicsObject = enumerator.Current as PhysicsObject;
					if (physicsObject != null && physicsObject.active && physicsObject.visible && physicsObject.grounded && (!(physicsObject is Holdable) || ((Holdable)physicsObject).duck == null))
					{
						if (base.isServerForObject && physicsObject.owner == null)
						{
							base.Fondle(physicsObject);
						}
						if (physicsObject.isServerForObject)
						{
							physicsObject.hSpeed = Rando.Float(-5f, 5f);
							physicsObject.vSpeed = -3f;
							Duck duck = physicsObject as Duck;
							if (duck != null)
							{
								Holdable holdObject = duck.holdObject;
								if (holdObject != null)
								{
									duck.ThrowItem(false);
									physicsObject.vSpeed -= 4f;
									physicsObject.hSpeed = duck.hSpeed * 0.8f;
									physicsObject.clip.Add(duck);
									duck.clip.Add(holdObject);
								}
								duck.GoRagdoll();
								if (holdObject != null)
								{
									duck.ragdoll.part1.clip.Add(holdObject);
									duck.ragdoll.part2.clip.Add(holdObject);
									duck.ragdoll.part3.clip.Add(holdObject);
									holdObject.clip.Add(duck.ragdoll.part1);
									holdObject.clip.Add(duck.ragdoll.part2);
									holdObject.clip.Add(duck.ragdoll.part3);
								}
							}
							Gun gun = physicsObject as Gun;
							if (gun != null)
							{
								gun.PressAction();
							}
						}
					}
				}
			}
*/