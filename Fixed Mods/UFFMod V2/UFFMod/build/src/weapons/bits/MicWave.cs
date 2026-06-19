using System;

namespace DuckGame.UFFMod
{
    internal class MicWave : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        private Sprite oval1;
        private Sprite oval2;
        private Sprite oval3;
        private int a;

        public MicWave(float xpos, float ypos)
            : base(xpos, ypos)
        {
            oval1 = new Sprite(Mod.GetPath<UffMod>("weapons\\ifYouAreReadingThisYouShouldKnowThatAlexIsAFuckWhoThinksYellowIsOrange"), 0f, 0f);
            oval2 = new Sprite(Mod.GetPath<UffMod>("weapons\\ifYouAreReadingThisYouShouldKnowThatAlexIsAFuckWhoThinksYellowIsOrange"), 0f, 0f);
            oval3 = new Sprite(Mod.GetPath<UffMod>("weapons\\ifYouAreReadingThisYouShouldKnowThatAlexIsAFuckWhoThinksYellowIsOrange"), 0f, 0f);
            oval1.CenterOrigin();
            oval2.CenterOrigin();
            oval3.CenterOrigin();
            oval2.angle = oval1.angle + Rando.Float((float)Math.PI / 8, (float)Math.PI / 4);
            oval3.angle = oval2.angle + Rando.Float((float)Math.PI / 8, (float)Math.PI / 4);
            oval1.scale = oval2.scale = oval3.scale = new Vec2(0.02f, 0.02f);
            a = Rando.Int(1) == 1 ? 1 : -1;
        }

        public override void Update()
        {
            oval1.angle = (oval1.angle > 2f * (float)Math.PI ? oval1.angle - 2f * (float)Math.PI : oval1.angle) + (a * 0.03f);
            oval2.angle = (oval2.angle > 2f * (float)Math.PI ? oval2.angle - 2f * (float)Math.PI : oval2.angle) + (-1f * a * 0.06f);
            oval3.angle = (oval3.angle > 2f * (float)Math.PI ? oval3.angle - 2f * (float)Math.PI : oval3.angle) + (a * 0.09f);
            oval1.xscale = MathHelper.Lerp(oval1.xscale, 0.444f, 0.12f);
            oval1.yscale = MathHelper.Lerp(oval1.yscale, 0.444f, 0.12f);
            oval2.xscale = MathHelper.Lerp(oval2.xscale, 0.667f, 0.09f);
            oval2.yscale = MathHelper.Lerp(oval2.yscale, 0.667f, 0.09f);
            oval3.xscale = MathHelper.Lerp(oval3.xscale, 1f, 0.06f);
            oval3.yscale = MathHelper.Lerp(oval3.yscale, 1f, 0.06f);
            oval1.alpha = MathHelper.Lerp(oval1.alpha, 0f, 0.08f);
            oval2.alpha = MathHelper.Lerp(oval2.alpha, 0f, 0.06f);
            oval3.alpha = MathHelper.Lerp(oval3.alpha, 0f, 0.04f);

            if (oval1.alpha <= 0.01f && oval2.alpha <= 0.01f && oval3.alpha == 0.01f)
                Level.Remove(this);

            base.Update();
        }

        public override void Draw()
        {
            Graphics.Draw(oval1, position.x, position.y);
            Graphics.Draw(oval2, position.x, position.y);
            Graphics.Draw(oval3, position.x, position.y);
            base.Draw();
        }
    }
}
