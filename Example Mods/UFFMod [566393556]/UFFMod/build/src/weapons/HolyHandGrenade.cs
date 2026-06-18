using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    // credits to Garoslaw

    [EditorGroup("uff|weapons|explosives")]
    public class HolyHandGrenade : Grenade
    {
        public StateBinding _realTimerStateBinding = new StateBinding("_realTimer");
        public StateBinding _detonationTriggerStateBinding = new StateBinding("_detonationTrigger");
        public StateBinding _animationIndexStateBinding = new StateBinding("netAnimationIndex");
        public StateBinding _frameStateBinding = new StateBinding("spriteFrame");
        public StateBinding netSFX_ahhStateBinding = (StateBinding)new NetSoundBinding("netSFX_ahh");

        public NetSoundEffect netSFX_ahh = new NetSoundEffect(new string[1]
        {
            Mod.GetPath<UffMod>("SFX\\ahh")
        });

        public float _realTimer;
        public int _detonationTrigger;

        private SpriteMap sprite;
        private float holyRadius;

        private byte netAnimationIndex
        {
            get
            {
                if (sprite == null)
                    return (byte)0;
                return (byte)sprite.animationIndex;
            }
            set
            {
                if (sprite == null || sprite.animationIndex == (int)value)
                    return;
                sprite.animationIndex = (int)value;
            }
        }

        public byte spriteFrame
        {
            get
            {
                if (sprite == null)
                    return (byte)0;
                return (byte)sprite._frame;
            }
            set
            {
                if (sprite == null)
                    return;
                sprite._frame = (int)value;
            }
        }

        public HolyHandGrenade(float xpos, float ypos) :
            base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\holyGrenade"), 10, 16);
            sprite.AddAnimation("glow", 0.1f, false, 1, 2, 3, 4, 5, 6, 7, 8, 9);
            graphic = sprite;
            center = new Vec2(5f, 8f);
            collisionOffset = new Vec2(-5f, -8f);
            collisionSize = new Vec2(10f, 16f);

            friction = 0.2f;
            bouncy = 0.6f;
            _detonationTrigger = 0;
            _realTimer = 3f;

            holyRadius = 64f; // I'll just leave that here for now.

            _editorName = "Holy Hand Grenade";
            _bio = "Bless this, O Lord, that with it thou mayst blow thine enemies to tiny bits, in thy mercy.";
        }

        public void Banish(MaterialThing materialThing)
        {
            if (materialThing is BlockGroup)
            {
                BlockGroup bg = materialThing as BlockGroup;
                foreach (Block bl in bg.blocks)
                    if (Collision.Circle(position, holyRadius, bl.rectangle))
                        Banish(bl);
                bg.Wreck();
            }
            else if (materialThing is Door || materialThing is VerticalDoor)
            {
                if (materialThing.owner == null)
                    Fondle(materialThing);
                materialThing.Destroy(new DTRocketExplosion(this));
                Level.Remove(materialThing);
            }
            else if (materialThing is Window)
            {
                if (materialThing.owner == null)
                    Fondle(materialThing);
                materialThing.Destroy(new DTImpact(this));
                Level.Remove(materialThing);
            }
            else if (materialThing is AutoBlock)
            {
                foreach (MaterialThing mT in Level.CheckCircleAll<MaterialThing>(materialThing.position, 16f))
                {
                    if (mT is BlockGroup)
                    {
                        BlockGroup b = mT as BlockGroup;
                        b.Wreck();
                    }
                    else if (mT is PhysicsObject && mT != this)
                    {
                        if (materialThing.isLocal && materialThing.owner == null)
                            Thing.Fondle(mT, DuckNetwork.localConnection);
                        ((PhysicsObject)mT).sleeping = false;
                        mT.vSpeed = -2f;
                    }
                }
                if (materialThing.owner == null)
                    Fondle(materialThing);
                HashSet<ushort> blocksToDestroy = new HashSet<ushort>();
                blocksToDestroy.Add((materialThing as AutoBlock).blockIndex);
                ((Block)materialThing).skipWreck = true;
                ((Block)materialThing).shouldWreck = true;
                if (Network.isActive && isLocal)
                    Send.Message(new NMDestroyBlocks(blocksToDestroy));
            }
            else if (materialThing is Duck)
                materialThing.Destroy(new DTIncinerate(this));
            else
            {
                if (materialThing.owner == null)
                    Fondle(materialThing);
                if (materialThing.Destroy(new DTImpact(this)))
                    Level.Remove(materialThing);
            }

        }

        public override void Update()
        {
            _timer = 99f;
            base.Update();

            if (!_pin)
                _realTimer -= 0.01f;

            if (!_pin && _detonationTrigger == 0 && _realTimer <= 1f)
            {
                SpawnHalo();

                _detonationTrigger++;
            }

            if (_realTimer <= 0f && _detonationTrigger == 1)
            {
                SpawnExplosion();

                foreach (BlockGroup blockGroup in Level.CheckCircleAll<BlockGroup>(position, holyRadius + 16f))
                    blockGroup.Wreck();

                foreach (MaterialThing materialThing in Level.CheckCircleAll<MaterialThing>(position, holyRadius))
                    Banish(materialThing);

                foreach (Ghost ghost in Level.CheckCircleAll<Ghost>(position, holyRadius))
                    ghost.Exorcise();

                _detonationTrigger++;
                _destroyed = true;
                Level.Remove(this);
            }
        }

        public void SpawnHalo()
        {
            if (isServerForObject)
            {
                Level.Add(new HHGHalo(position.x, position.y - 12f, this));
                netSFX_ahh.Play();
            }
        }

        public void SpawnExplosion()
        {
            if (isServerForObject)
                for (double d = 0; d < 2 * Math.PI; d += Math.PI / 8)
                {
                    Level.Add(new GlobalExplosion(x + (Rando.Float(0f, 32f) * (float)Math.Cos(d)), y + (Rando.Float(0f, 32f) * (float)Math.Sin(d))));
                    Level.Add(new GlobalExplosion(x + (Rando.Float(32f, 64f) * (float)Math.Cos(d)), y + (Rando.Float(32f, 64f) * (float)Math.Sin(d))));
                }

            SFX.Play("explode");
        }

        public override void Draw()
        {
            if (_detonationTrigger >= 1 && !sprite.currentAnimation.Equals("glow"))
                sprite.SetAnimation("glow");
            base.Draw();
        }

        public override void OnNetworkBulletsFired(Vec2 pos)
        {
            // do nothing
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            SFX.Play(Mod.GetPath<UffMod>("SFX\\anvilTing"), 0.3f, 0.8f);
            base.OnSolidImpact(with, from);
        }
    }
}
