using System.Collections.Generic;
using System.Linq;
using System;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|stuff|blocks")]
    public class ConveyorBelt : Block, IDontMove, IPathNodeBlocker, IPlatform
    {
        public StateBinding _positionBinding = new StateBinding("position");

        public EditorProperty<float> speed;
        public EditorProperty<int> length;

        private IList<ConveyerPart> parts = new List<ConveyerPart>();

        private float animUpdate;

        public ConveyorBelt(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _editorName = "Conveyor Belt";
            speed = new EditorProperty<float>(1f, this, 0f, 2.5f, 0.25f);
            length = new EditorProperty<int>(1, this, 1f, 31f, 1f);
            SpriteMap sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\blocks\\conveyorBelt"), 16, 16);
            graphic = sprite;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            depth = 0.5f;
            animUpdate = 0;
            CalculateWidth();
        }

        private void CalculateWidth()
        {
            parts.Clear();
            collisionSize = new Vec2(16f * length, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            for (int i = 0; i < length; i++)
            {
                ConveyorType type;
                if (length == 1)
                    type = ConveyorType.One;
                else
                {
                    if (i == 0)
                        type = ConveyorType.Left;
                    else if (i == length - 1)
                        type = ConveyorType.Right;
                    else
                        type = ConveyorType.Middle;
                }
                parts.Add(new ConveyerPart(type, i));
            }
            UpdateAnimation();
        }

        public override void EditorPropertyChanged(object property)
        {
            CalculateWidth();
        }

        public override void Initialize()
        {
            graphic = null;
            CalculateWidth();
        }

        public override void Update()
        {
            foreach (PhysicsObject physicsObject in Level.CheckRectAll<PhysicsObject>(topLeft - new Vec2(-1f, 2f), bottomRight - new Vec2(1f, 12f)))
            {
                if (physicsObject.isServerForObject && physicsObject.grounded)
                {
                    if (physicsObject is RagdollPart)
                        physicsObject.hSpeed = MathHelper.Lerp(physicsObject.hSpeed, offDir * 1.3f * speed, 0.2f);
                    else
                    {
                        Block block = Level.CheckRect<Block>(
                            offDir > 0 ? (physicsObject.topRight + new Vec2(0f, 1f)) : (physicsObject.topLeft - new Vec2(speed, -1f)),
                            offDir > 0 ? (physicsObject.bottomRight + new Vec2(speed, -1f)) : (physicsObject.bottomLeft - new Vec2(0f, 1f)));
                        if (block == null)
                            physicsObject.x += offDir * speed;
                        else
                            physicsObject.x = (offDir > 0 ? block.left : block.right) - offDir * ((physicsObject.offDir == offDir) ? physicsObject.collisionSize.x + physicsObject.collisionOffset.x : Math.Abs(physicsObject.collisionOffset.x));

                        if (!Level.CheckRectAll<PhysicsObject>(topLeft - new Vec2(-1f, 2f), bottomRight - new Vec2(1f, 12f)).Contains(physicsObject)
                            && ((offDir > 0 && physicsObject.x >= right)
                            || (offDir < 0 && physicsObject.x <= left))
                            && ((offDir < 0 && Level.CheckRect<Block>(topLeft - new Vec2(physicsObject.collisionSize.x - 1f, 0f), topLeft + new Vec2(-1f, 1f)) == null)
                            || (offDir > 0 && Level.CheckRect<Block>(topRight + new Vec2(1f, 0f), topRight + new Vec2(physicsObject.collisionSize.x - 1f, 1f)) == null)))
                        {
                            physicsObject.x = (offDir > 0 ? right : left) + offDir * ((physicsObject.offDir == offDir) ? Math.Abs(physicsObject.collisionOffset.x) : physicsObject.collisionSize.x + physicsObject.collisionOffset.x);
                            if ((offDir > 0 && physicsObject.hSpeed < 0f) || (offDir < 0 && physicsObject.hSpeed > 0f))
                                physicsObject.hSpeed = 0f;
                            physicsObject.vSpeed = 1f;
                        }
                    }
                }
            }

            animUpdate += speed / 2f;
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            while (animUpdate >= 1)
            {
                foreach (ConveyerPart part in parts)
                {
                    if (part.type == ConveyorType.One)
                        part.sprite.frame = part.sprite.frame > 12 ? (part.sprite.frame - 1) : 15; // one
                    else if (part.type == ConveyorType.Left)
                        part.sprite.frame = part.sprite.frame > 0 ? (part.sprite.frame - 1) : 3; // left
                    else if (part.type == ConveyorType.Right)
                        part.sprite.frame = part.sprite.frame > 8 ? (part.sprite.frame - 1) : 11; // right
                    else
                        part.sprite.frame = part.sprite.frame > 4 ? (part.sprite.frame - 1) : 7; // middle
                }
                animUpdate -= 1;
            }
        }

        public override void Draw()
        {
            foreach(ConveyerPart part in parts)
            {
                part.sprite.flipH = offDir < 0;
                Graphics.Draw((Sprite)part.sprite, x - offDir * (8f - part.position * 16f), y - 8f);
            }
        }
    }

    internal class ConveyerPart
    {
        public ConveyorType type;
        public SpriteMap sprite;
        public int position;

        public ConveyerPart(ConveyorType ct, int p)
        {
            position = p;
            type = ct;
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\blocks\\conveyorBelt"), 16, 16);
            sprite.frame += (int)type;
        }
    }

    enum ConveyorType : int
    {
        Left = 0,
        Middle = 4,
        Right = 8,
        One = 12
    }
}
