using System;
using System.Collections.Generic;

namespace DuckGame.UFFMod
{
    [EditorGroup("uff|stuff|hazards")]
    public class Firebar : Thing
    {
        public EditorProperty<int> initialAngle = new EditorProperty<int>(0, null, 0f, 360f, 1f);
        public EditorProperty<int> length = new EditorProperty<int>(5, null, 1f, 32f, 1f);
        public EditorProperty<float> speed = new EditorProperty<float>(2f, null, 0f, 12f, 0.25f);
        public EditorProperty<bool> clockwise = new EditorProperty<bool>(true);
        public EditorProperty<bool> hasBlock = new EditorProperty<bool>(false);

        private IList<FirebarBall> fireballs = new List<FirebarBall>();
        private float barAngle;
        private GreyBlock greyBlock;

        public Firebar(float xpos, float ypos)
            : base(xpos, ypos)
        {
            graphic = (Sprite)new SpriteMap(Mod.GetPath<UffMod>("stuff\\hazards\\firebarBit"), 6, 6);
            center = new Vec2(3f, 3f);
            collisionOffset = new Vec2(0f, 0f);
            collisionSize = new Vec2(0f, 0f);
            _canFlip = false;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
            {
                graphic = (Sprite)new SpriteMap(Mod.GetPath<UffMod>("stuff\\hazards\\firebarBit"), 6, 6);
                return;
            }

            graphic = null;

            if (hasBlock)
            {
                greyBlock = new GreyBlock(x, y);
                Level.Add(greyBlock);
            }

            FirebarBall fb;
            for (int i = 0; i < length; i++)
            {
                fb = new FirebarBall(x, y - (6f * (i)), i, this);
                Level.Add(fb);
                fireballs.Add(fb);
            }

            barAngle = initialAngle;
        }

        public override void Terminate()
        {
            if (greyBlock != null)
                Level.Remove(greyBlock);
            foreach (FirebarBall firebarBall in fireballs)
                Level.Remove(firebarBall);
            base.Terminate();
        }

        public override void Update()
        {
            foreach(FirebarBall ball in fireballs)
            {
                ball.x = x + (ball.order * 6f * (float)Math.Cos((Math.PI * barAngle) / 180));
                ball.y = y + (ball.order * 6f * (float)Math.Sin((Math.PI * barAngle) / 180));
            }

            if (barAngle >= 360)
                barAngle -= 360;
            if (barAngle <= -360)
                barAngle += 360;

            barAngle += clockwise ? (float)speed : (float)-speed;
            base.Update();
        }
    }
}
