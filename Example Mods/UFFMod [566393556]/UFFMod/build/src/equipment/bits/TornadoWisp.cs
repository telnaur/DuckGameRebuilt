namespace DuckGame.UFFMod
{
    internal class TornadoWisp : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        private SpriteMap sprite;

        public TornadoWisp(float xpos, float ypos)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\tornadoJump"), 20, 20);
            sprite.AddAnimation("*schwish*", 0.5f, false, 0, 1, 2, 3);
            sprite.SetAnimation("*schwish*");
            graphic = sprite;
            center = new Vec2(10f, 10f);
            depth = 2f;
        }

        public override void Update()
        {
            if (sprite.finished)
                Level.Remove(this);
        }
    }
}
