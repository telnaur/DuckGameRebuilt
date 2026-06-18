using System;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|explosives")]
    public class TrollBomb : Grenade
    {
        public StateBinding _frameStateBinding = new StateBinding("spriteFrame");

        private SpriteMap sprite;
        private Vec2 fusePos;
        private bool flash;
        private int timeSinceFlash;

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

        public TrollBomb(float xpos, float ypos) :
            base(xpos, ypos)
        {
            _editorName = "Troll Bomb";
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\trollBomb"), 16, 16);
            graphic = sprite;
            center = new Vec2(10f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            _pin = false;
            timeSinceFlash = 0;
        }

        public override void Update()
        {
            if (_timer >= 0f)
                sprite.frame = 8 - (int)Math.Floor((20f / 3f) * _timer);
            else
                sprite.frame = 8;

            if (flash)
            {
                if(sprite.frame < 9)
                    sprite.frame += 9;
                timeSinceFlash--;
            }
            else
            {
                if (sprite.frame >= 9)
                    sprite.frame -= 9;
                timeSinceFlash++;
            }

            if (timeSinceFlash >= 30 * _timer)
            {
                flash = true;
                timeSinceFlash = 0;
            }
            else if (timeSinceFlash <= -3)
                flash = false;

            if (yscale < 1.15f)
            {
                xscale -= 0.005f;
                yscale += 0.01f;
            }
            else
            {
                xscale = yscale = 1f;
            }

            int currentFrame = sprite.frame >= 9 ? sprite.frame - 9 : sprite.frame;
            switch (currentFrame)
            {
                default:
                    fusePos = new Vec2(0f, 4f);
                    break;

                case 1:
                    fusePos = new Vec2(0f, 5f);
                    break;

                case 2:
                    fusePos = new Vec2(1f, 6f);
                    break;

                case 3:
                    fusePos = new Vec2(2f, 5f);
                    break;

                case 4:
                    fusePos = new Vec2(2f, 4f);
                    break;

                case 5:
                    fusePos = new Vec2(2f, 3f);
                    break;

                case 6:
                    fusePos = new Vec2(3f, 2f);
                    break;

                case 7:
                    fusePos = new Vec2(4f, 2f);
                    break;

                case 8:
                    fusePos = new Vec2(5f, 3f);
                    break;
            }
            fusePos -= center - new Vec2(1f, 1f);
            fusePos.x *= offDir;
            fusePos = fusePos.Rotate(angle, Vec2.Zero);
            Level.Add(Spark.New(x + fusePos.x, y + fusePos.y, -fusePos.normalized));

            base.Update();
        }
    }
}
