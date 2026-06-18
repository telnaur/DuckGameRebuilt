namespace DuckGame.UFFMod
{
    [EditorGroup("uff|stuff|blocks")]
    public class POWBlock : BaseBox
    {
        public StateBinding _netPOWedStateBinding = new StateBinding("_netPOWed");
        
        public byte _netPOWed;

        private byte localPOWed;

        public POWBlock(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _editorName = "POW Block";
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\blocks\\pow"), 16, 16);
            graphic = sprite;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            depth = 0.5f;
            timesUsed = 0;
            _canFlip = false;
            _netHitSound = new NetSoundEffect(new string[1]
            {
              Mod.GetPath<UffMod>("SFX\\POW")
            });
        }

        public override void Activate(MaterialThing with)
        {
            if (Network.isActive)
                _netPOWed++;
            DoPOW();
            base.Activate(with);
        }

        public override void Update()
        {
            if (_netPOWed != localPOWed)
            {
                localPOWed = _netPOWed;
                DoPOW();
            }
            base.Update();
        }

        private void DoPOW()
        {
            foreach (Thing thing in Level.current.things)
            {
                PhysicsObject physicsObject = thing as PhysicsObject;

                if (physicsObject == null || !physicsObject.active || !physicsObject.visible || !physicsObject.grounded || (physicsObject is Holdable && ((Holdable)physicsObject).duck != null))
                    continue;

                if (isServerForObject && physicsObject.owner == null)
                    Fondle(physicsObject);

                if (physicsObject.isServerForObject)
                {
                    physicsObject.hSpeed = Rando.Float(-5f, 5f);
                    physicsObject.vSpeed = -3f;

                    Duck theDuck = physicsObject as Duck;
                    if (theDuck != null)
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
                    Gun gun = physicsObject as Gun;
                    if (gun != null)
                        gun.PressAction();
                }
            }
        }
    }
}