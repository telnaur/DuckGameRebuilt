using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class ATCannon : AmmoType
    {
        public ATCannon()
        {
            this.accuracy = 1f;
            this.range = 2500f;
            this.penetration = 0.4f;
            this.bulletSpeed = 8f;
            this.bulletThickness = 2f;
            this.sprite = new Sprite(Mod.GetPath<DuckGame.MyMod.MyMod>("cannonball"), 0.0f, 0.0f);
            this.sprite.CenterOrigin();
            this.speedVariation = 0.0f;
            this.flawlessPipeTravel = true;
            this.affectedByGravity = true;
            this.weight = 3f;
            this.ownerSafety = 4;
        }

        public override void OnHit(bool destroyed, Bullet b)
        {
            if (!b.isLocal)
                return;
            if (destroyed)
            {
                RumbleManager.AddRumbleEvent(b.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium, RumbleType.Gameplay));
                new ATMissileShrapnel().MakeNetEffect(b.position, false);
                Random random = (Random)null;
                if (Network.isActive && b.isLocal)
                {
                    random = Rando.generator;
                    Rando.generator = new Random(NetRand.currentSeed);
                }
                List<Bullet> varBullets = new List<Bullet>();
                for (int index = 0; index < 12; ++index)
                {
                    float num = (float)((double)index * 30.0 - 10.0) + Rando.Float(20f);
                    ATMissileShrapnel atMissileShrapnel = new ATMissileShrapnel();
                    atMissileShrapnel.range = 15f + Rando.Float(5f);
                    Vec2 vec2 = new Vec2((float)Math.Cos((double)Maths.DegToRad(num)), (float)Math.Sin((double)Maths.DegToRad(num)));
                    Bullet bullet = new Bullet(b.x + vec2.x * 8f, b.y - vec2.y * 8f, (AmmoType)atMissileShrapnel, num, (Thing)null, false, -1f, false, true);
                    bullet.firedFrom = (Thing)b;
                    varBullets.Add(bullet);
                    Level.Add((Thing)bullet);
                    Level.Add((Thing)Spark.New(b.x + Rando.Float(-8f, 8f), b.y + Rando.Float(-8f, 8f), vec2 + new Vec2(Rando.Float(-0.1f, 0.1f), Rando.Float(-0.1f, 0.1f)), 0.02f));
                    Level.Add((Thing)SmallSmoke.New(b.x + vec2.x * 8f + Rando.Float(-8f, 8f), b.y + vec2.y * 8f + Rando.Float(-8f, 8f)));
                }
                if (Network.isActive && b.isLocal)
                {
                    Send.Message((NetMessage)new NMFireGun((Gun)null, varBullets, (byte)0, false, (byte)4, false), NetMessagePriority.ReliableOrdered, (NetworkConnection)null);
                    varBullets.Clear();
                }
                if (Network.isActive && b.isLocal)
                    Rando.generator = random;
                ATMissile.DestroyRadius(b.position, 50f, (Thing)b, false);
            }
            base.OnHit(destroyed, b);
        }

        public static int DestroyRadius(Vec2 pPosition, float pRadius, Thing pBullet, bool pExplode = false)
        {
            foreach (Window window in Level.CheckCircleAll<Window>(pPosition, pRadius - 20f))
            {
                Thing.Fondle((Thing)window, DuckNetwork.localConnection);
                if (Level.CheckLine<Block>(pPosition, window.position, (Thing)window) == null)
                    window.Destroy((DestroyType)new DTImpact(pBullet));
            }
            foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(pPosition, pRadius + 30f))
            {
                if (pBullet.isLocal && pBullet.owner == null)
                    Thing.Fondle((Thing)physicsObject, DuckNetwork.localConnection);
                if ((double)(physicsObject.position - pPosition).length < 30.0)
                    physicsObject.Destroy((DestroyType)new DTImpact(pBullet));
                physicsObject.sleeping = false;
                physicsObject.vSpeed = -2f;
            }
            int num = 0;
            HashSet<ushort> varBlocks = new HashSet<ushort>();
            foreach (BlockGroup blockGroup1 in Level.CheckCircleAll<BlockGroup>(pPosition, pRadius))
            {
                if (blockGroup1 != null)
                {
                    BlockGroup blockGroup2 = blockGroup1;
                    List<Block> blockList = new List<Block>();
                    foreach (Block block in blockGroup2.blocks)
                    {
                        if (Collision.Circle(pPosition, pRadius - 22f, block.rectangle))
                        {
                            block.shouldWreck = true;
                            if (block is AutoBlock && !(block as AutoBlock).indestructable)
                            {
                                varBlocks.Add((block as AutoBlock).blockIndex);
                                if (pExplode && num % 10 == 0)
                                {
                                    Level.Add((Thing)new ExplosionPart(block.x, block.y, true));
                                    Level.Add((Thing)SmallFire.New(block.x, block.y, Rando.Float(-2f, 2f), Rando.Float(-2f, 2f), false, (MaterialThing)null, true, (Thing)null, false));
                                }
                                ++num;
                            }
                        }
                    }
                    blockGroup2.Wreck();
                }
            }
            foreach (Block block in Level.CheckCircleAll<Block>(pPosition, pRadius - 22f))
            {
                if (block is AutoBlock && !(block as AutoBlock).indestructable)
                {
                    block.skipWreck = true;
                    block.shouldWreck = true;
                    varBlocks.Add((block as AutoBlock).blockIndex);
                    if (pExplode)
                    {
                        if (num % 10 == 0)
                        {
                            Level.Add((Thing)new ExplosionPart(block.x, block.y, true));
                            Level.Add((Thing)SmallFire.New(block.x, block.y, Rando.Float(-2f, 2f), Rando.Float(-2f, 2f), false, (MaterialThing)null, true, (Thing)null, false));
                        }
                        ++num;
                    }
                }
                else if (block is Door || block is VerticalDoor)
                {
                    Level.Remove((Thing)block);
                    block.Destroy((DestroyType)new DTRocketExplosion((Thing)null));
                }
            }
            if (Network.isActive && (pBullet.isLocal || pBullet.isServerForObject) && varBlocks.Count > 0)
                Send.Message((NetMessage)new NMDestroyBlocks(varBlocks));
            foreach (ILight light in Level.current.things[typeof(ILight)])
                light.Refresh();
            return varBlocks.Count;
        }
    }
}
