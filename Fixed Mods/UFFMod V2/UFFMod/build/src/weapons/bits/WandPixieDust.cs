namespace DuckGame.UFFMod
{
    // wizard hat particles
    internal class WandPixieDust : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        private SpriteMap sprite;

        public WandPixieDust(float xpos, float ypos)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\wandBits"), 5, 5);
            sprite.AddAnimation("*kirakira1*", 1f, false, 0, 1, 2, 3, 4, 5, 6, 7);
            sprite.AddAnimation("*kirakira2*", 1f, false, 2, 3, 4, 5, 6, 7);
            sprite.AddAnimation("*kirakira3*", 1f, false, 4, 5, 6, 7);
            int rand = Rando.Int(1, 3);
            if (rand == 1)
                sprite.SetAnimation("*kirakira1*");
            else if (rand == 2)
                sprite.SetAnimation("*kirakira2*");
            else if (rand == 3)
                sprite.SetAnimation("*kirakira3*");
            graphic = sprite;
            sprite.speed = 0.4f + Rando.Float(0.2f);
            xscale = 0.8f + Rando.Float(0.4f);
            yscale = xscale;
            center = new Vec2(3f, 3f);
            depth = Rando.Float(0.2f, 1f);
            vSpeed = Rando.Float(0.6f, 1.2f);
        }

        public override void Update()
        {
            y += vSpeed;
            if (sprite.finished)
                Level.Remove((Thing)this);
        }
    }
}
