using System;

namespace DuckGame.UFFMod
{
    public class LeafblowerGust : Thing
    {
        public StateBinding _theBlowerStateBinding = new StateBinding("_theBlower");
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        public Leafblower _theBlower;
        private SpriteMap sprite;

        public LeafblowerGust(float xpos, float ypos, Leafblower lb)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\leafblowerGust"), 46, 23);
            sprite.AddAnimation("*schwoo*", 0.5f, true, 0, 1, 2, 3);
            sprite.SetAnimation("*schwoo*");
            graphic = sprite;
            center = new Vec2(23f, 12f);
            depth = 0.3f;
            _theBlower = lb;
        }

        public override void Update()
        {
            if (_theBlower == null || !_theBlower._isFiring)
            {
                visible = false;
                return;
            }

            visible = true;

            offDir = _theBlower.offDir;
            if (offDir < 0)
                sprite.flipH = true;
            else
                sprite.flipH = false;

            x = _theBlower.x + _theBlower.offDir * 32f;
            y = _theBlower.y;

            foreach (Ghost ghost in Level.CheckRectAll<Ghost>(new Vec2(x - 18f, y - 11f), new Vec2(x + 18f, y + 12f)))
            {
                if (Level.CheckLine<Block>(_theBlower.position, ghost.position, ghost) == null)
                {
                    ghost.hSpeed += offDir * (float)Math.Pow((ghost.position - position).length, 1 / 3);
                    ghost.vSpeed -= 0.3f;
                }
            }

            foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(new Vec2(x - 18f, y - 11f), new Vec2(x + 18f, y + 12f)))
            {
                if ((Level.CheckLine<Block>(_theBlower.position, physicsObject.position, physicsObject) == null) && physicsObject != _theBlower && physicsObject != _theBlower.owner)
                {
                    physicsObject.hSpeed += offDir * 3f * (float)Math.Pow((physicsObject.position - position).length, 1 / 3) / physicsObject.weight;
                    if (physicsObject.weight <= 10f)
                        physicsObject.vSpeed -= 1.2f / (float)Math.Sqrt(physicsObject.weight);

                    if (physicsObject is Grenade)
                        ((Grenade)physicsObject).PressAction();

                    if (physicsObject is Duck)
                    {
                        Duck theDuck = physicsObject as Duck;
                        if (theDuck.isServerForObject)
                        {
                            Holdable heldItem = theDuck.holdObject;
                            if (heldItem != null)
                            {
                                theDuck.ThrowItem(false);
                                physicsObject.vSpeed -= 4f;
                                physicsObject.hSpeed = theDuck.hSpeed * 0.8f;
                                physicsObject.clip.Add(theDuck);
                                theDuck.clip.Add(heldItem);
                            }
                            theDuck.GoRagdoll();
                            if (heldItem != null)
                            {
                                theDuck.ragdoll.part1.clip.Add(heldItem);
                                theDuck.ragdoll.part2.clip.Add(heldItem);
                                theDuck.ragdoll.part3.clip.Add(heldItem);
                                heldItem.clip.Add(theDuck.ragdoll.part1);
                                heldItem.clip.Add(theDuck.ragdoll.part2);
                                heldItem.clip.Add(theDuck.ragdoll.part3);
                            }
                        }
                    }
                }
            }
        }
    }
}
