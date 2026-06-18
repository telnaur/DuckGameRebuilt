using System;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|stuff|hazards")]
    public class Reaper : Ghost
    {
        private float indicatorAngleOffset;

        public Reaper(float xpos, float ypos, bool startSpawned = false, bool waitForPlayer = false)
            : base(xpos, ypos, startSpawned, waitForPlayer)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\hazards\\reaper"), 20, 24);
            sprite.AddAnimation("haunt", 0.125f, true, 0, 1, 2, 3);
            sprite.SetAnimation("haunt");
            graphic = sprite;
            reaper = true;
        }

        public override void Draw()
        {
            if (_controlDuck != null)
            {
                Color dotColour = _controlDuck.profile.persona.colorUsable * alpha;
                float mod = 1f;
                for (float f = indicatorAngleOffset; f < indicatorAngleOffset + (2f * (float)Math.PI); f += (float)Math.PI / 4f)
                {
                    Vec2 dotTopLeft = new Vec2(x + 16f * (float)Math.Cos(f) - mod, y + 16f * (float)Math.Sin(f) - mod);
                    Vec2 dotBottomRight = dotTopLeft + new Vec2(2f * mod, 2f * mod);
                    Graphics.DrawRect(dotTopLeft, dotBottomRight, dotColour, depth);
                    mod = mod == 1f ? 2f : 1f;
                }

                indicatorAngleOffset = (indicatorAngleOffset + (float)Math.PI / 32f) % (2f * (float)Math.PI);
            }

            base.Draw();
        }
    }
}
