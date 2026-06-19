using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    internal class ATWorldEater : AmmoType
    {
        public bool angleShot = true;

        public ATWorldEater()
        {
            accuracy = 1f;
            range = 1000f;
            penetration = 1f;
            bulletSpeed = 20f;
            bulletThickness = 0.3f;
            bulletType = typeof(SerpentLaser);
        }

        public override void OnHit(bool destroyed, Bullet b)
        {
            if (destroyed && Level.current.CollisionPoint<Reflector>(b.position) == null && Level.current.CollisionPoint<ReflectorBlock>(b.position) == null)
            {
                Level.Add(new GlobalVaporize(b.x, b.y));

                if (b.isLocal)
                {
                    foreach (BlockGroup blockGroup in Level.CheckCircleAll<BlockGroup>(b.position, 16f))
                        blockGroup.Wreck();
                    foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(b.position, 2f))
                        if(materialThing.solid && materialThing.thickness > 0f && !(materialThing is Gun) && !(materialThing is Equipment) && !(materialThing is RagdollPart))
                            EatWorlds(materialThing, b);
                }
            }

            if(b.isLocal)
                base.OnHit(destroyed, b);
        }

        private void EatWorlds(MaterialThing materialThing, Bullet b)
        {
            if (materialThing is BlockGroup)
            {
                BlockGroup bg = materialThing as BlockGroup;
                foreach (Block bl in bg.blocks)
                    if (Collision.Circle(b.position, 2f, bl.rectangle))
                        EatWorlds(bl, b);
                bg.Wreck();
            }
            else if (materialThing is AutoBlock)
            {
                foreach (MaterialThing mT in Level.CheckCircleAll<MaterialThing>(materialThing.position, 24f))
                {
                    if (mT is BlockGroup)
                    {
                        BlockGroup bg = mT as BlockGroup;
                        bg.Wreck();
                    }
                    else if (mT is PhysicsObject)
                    {
                        if (mT.owner == null)
                            Thing.Fondle(mT, DuckNetwork.localConnection);
                        ((PhysicsObject)mT).sleeping = false;
                        mT.vSpeed = -2f;
                    }
                }
                HashSet<ushort> blocksToDestroy = new HashSet<ushort>();
                blocksToDestroy.Add((materialThing as AutoBlock).blockIndex);
                ((Block)materialThing).skipWreck = true;
                ((Block)materialThing).shouldWreck = true;
                if (Network.isActive && b.isLocal)
                    Send.Message(new NMDestroyBlocks(blocksToDestroy));
            }
            else
            {
                if (materialThing.owner == null)
                    Thing.Fondle(materialThing, DuckNetwork.localConnection);
                if (materialThing.Destroy(new DTImpact(b)))
                    Level.Remove(materialThing);
            }
        }
    }
}
