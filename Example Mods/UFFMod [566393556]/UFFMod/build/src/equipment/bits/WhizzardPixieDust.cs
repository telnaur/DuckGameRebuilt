namespace DuckGame.UFFMod
{
    // wizard hat particles
    internal class WhizzardPixieDust : Thing
    {
        private SpriteMap sprite;
        private bool stationary;

        public WhizzardPixieDust(float xpos, float ypos, bool stationary = false)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\whizzardPixieDust"), 7, 7);
            sprite.AddAnimation("*kirakira*", 1f, false, 0, 1, 2, 3, 4);
            sprite.SetAnimation("*kirakira*");
            graphic = sprite;
            sprite.speed = 0.4f + Rando.Float(0.2f);
            xscale = 0.8f + Rando.Float(0.4f);
            yscale = xscale;
            center = new Vec2(3f, 3f);
            depth = Rando.Float(0.2f, 1f);
            vSpeed = Rando.Float(1.2f, 1.4f);
            this.stationary = stationary;
        }

        public override void Update()
        {
            if (!stationary)
                y += vSpeed;
            if (sprite.finished)
                Level.Remove(this);
        }
    }
}
