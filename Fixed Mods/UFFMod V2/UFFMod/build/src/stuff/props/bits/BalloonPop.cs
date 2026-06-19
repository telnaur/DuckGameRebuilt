namespace DuckGame.UFFMod
{
    internal class BalloonPop : Thing
    {
        private SpriteMap sprite;

        public BalloonPop(float xpos, float ypos)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\props\\balloon"), 15, 15);
            sprite.AddAnimation("pop", 0.25f, false, 2, 3);
            sprite.SetAnimation("pop");
            graphic = sprite;
            center = new Vec2(7f, 7f);
            depth = -0.5f;
        }

        public override void Update()
        {
            if (sprite.finished)
                Level.Remove(this);
        }
    }
}
