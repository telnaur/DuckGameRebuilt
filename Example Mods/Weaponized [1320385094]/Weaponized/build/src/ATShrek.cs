using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuckGame;

namespace MyMod.src
{
    public class ATShrek : AmmoType
    {

        public ATShrek()
        {
            this.accuracy = 0.95f;
            this.range = 2000f;
            this.penetration = 0.4f;
            this.bulletSpeed = 3.2f;
            this.bulletThickness = 5f;
            this.sprite = new Sprite(Mod.GetPath<DuckGame.MyMod.MyMod>("shrek"), 0.0f, 0.0f);
            this.sprite.CenterOrigin();
            this.bulletColor = Color.DarkOliveGreen;
            this.impactPower = 12f;


        }

        public override void PopShell(float x, float y, int dir)
        {
            slomoShell slomoShell = new slomoShell(x, y);
            slomoShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)slomoShell);

        }

        public override void OnHit(bool destroyed, Bullet b)
        {
            if (!b.isLocal)
                return;
            if (destroyed)
            {
                new ATShrekShrapnel().MakeNetEffect(b.position, false);
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
                    ATShrekShrapnel ATShrekShrapnel = new ATShrekShrapnel();
                    ATShrekShrapnel.range = 65f + Rando.Float(5f);
                    Vec2 vec2 = new Vec2((float)Math.Cos((double)Maths.DegToRad(num)), (float)Math.Sin((double)Maths.DegToRad(num)));
                    Bullet bullet = new Bullet(b.x + vec2.x * 8f, b.y - vec2.y * 8f, (AmmoType)ATShrekShrapnel, num, (Thing)null, false, -1f, false, true);
                    bullet.firedFrom = (Thing)b;
                    varBullets.Add(bullet);
                    Level.Add((Thing)bullet);
                    Level.Add((Thing)Spark.New(b.x + Rando.Float(-8f, 8f), b.y + Rando.Float(-8f, 8f), vec2 + new Vec2(Rando.Float(-0.1f, 0.1f), Rando.Float(-0.1f, 0.1f)), 0.02f));
                    Level.Add((Thing)Spark.New(b.x + Rando.Float(-8f, 8f), b.y + Rando.Float(-8f, 8f), vec2 + new Vec2(Rando.Float(-0.1f, 0.1f), Rando.Float(-0.1f, 0.1f)), 0.02f));
                    Level.Add((Thing)shrekSmallSmoke.New(b.x + vec2.x * 8f + Rando.Float(-8f, 8f), b.y + vec2.y * 8f + Rando.Float(-8f, 8f)));
                    Level.Add((Thing)shrekSmallSmoke.New(b.x + vec2.x * 8f + Rando.Float(-8f, 8f), b.y + vec2.y * 8f + Rando.Float(-8f, 8f)));
                }
                if (Network.isActive && b.isLocal)
                {
                    Send.Message((NetMessage)new NMFireGun((Gun)null, varBullets, (byte)0, false, (byte)4, false), NetMessagePriority.ReliableOrdered, (NetworkConnection)null);
                    varBullets.Clear();
                }
                if (Network.isActive && b.isLocal)
                    Rando.generator = random;
                foreach (Window window in Level.CheckCircleAll<Window>(b.position, 65f))
                {
                    if (b.isLocal)
                        Thing.Fondle((Thing)window, DuckNetwork.localConnection);
                    if (Level.CheckLine<Block>(b.position, window.position, (Thing)window) == null)
                        window.Destroy((DestroyType)new DTImpact((Thing)b));
                }
                foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(b.position, 65f))
                {
                    if (b.isLocal && b.owner == null)
                        Thing.Fondle((Thing)physicsObject, DuckNetwork.localConnection);
                    if ((double)(physicsObject.position - b.position).length < 65.0)
                        physicsObject.Destroy((DestroyType)new DTImpact((Thing)b));
                    physicsObject.sleeping = false;
                    physicsObject.vSpeed = -2f;
                }
                HashSet<ushort> varBlocks = new HashSet<ushort>();
                foreach (BlockGroup blockGroup1 in Level.CheckCircleAll<BlockGroup>(b.position, 65f))
                {
                    if (blockGroup1 != null)
                    {
                        BlockGroup blockGroup2 = blockGroup1;
                        List<Block> blockList = new List<Block>();
                        foreach (Block block in blockGroup2.blocks)
                        {
                            if (Collision.Circle(b.position, 65f, block.rectangle))
                            {
                                block.shouldWreck = true;
                                if (block is AutoBlock)
                                    varBlocks.Add((block as AutoBlock).blockIndex);
                            }
                        }
                        blockGroup2.Wreck();
                    }
                }
                foreach (Block block in Level.CheckCircleAll<Block>(b.position, 65f))
                {
                    if (block is AutoBlock)
                    {
                        block.skipWreck = true;
                        block.shouldWreck = true;
                        if (block is AutoBlock)
                            varBlocks.Add((block as AutoBlock).blockIndex);
                    }
                    else if (block is Door || block is VerticalDoor)
                    {
                        Level.Remove((Thing)block);
                        block.Destroy((DestroyType)new DTRocketExplosion((Thing)null));
                    }
                }
                if (Network.isActive && b.isLocal && varBlocks.Count > 0)
                    Send.Message((NetMessage)new NMDestroyBlocks(varBlocks));
            }
            base.OnHit(destroyed, b);
        }
    }
}