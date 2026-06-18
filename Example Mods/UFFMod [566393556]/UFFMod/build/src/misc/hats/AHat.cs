namespace DuckGame.UFFMod
{
    internal class AHat : TeamHat
    {
        public AHat(float x, float y, Team t)
            : base(x, y, t)
        {
        }

        public override void Update()
        {
            if (!(Level.current is GameLevel
                || Level.current is Editor
                || Level.current is TeamSelect2))
                return;

            if (equippedDuck != null && equippedDuck.inputProfile.Pressed(Triggers.Quack))
            {
                WhizzardPixieDust pixieDust = new WhizzardPixieDust(x - (equippedDuck.sliding ? -2f * equippedDuck.offDir : 5f * equippedDuck.offDir), y + 2f, true);
                Level.Add(pixieDust);
                Fondle(pixieDust);
            }

            base.Update();
        }
    }
}
