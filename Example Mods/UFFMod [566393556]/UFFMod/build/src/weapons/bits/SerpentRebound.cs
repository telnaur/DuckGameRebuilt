namespace DuckGame.UFFMod
{
    internal class SerpentRebound : Thing
    {
        private Tex2D _rebound = Content.Load<Tex2D>(Mod.GetPath<UffMod>("weapons\\serpentRebound.png"));

        public SerpentRebound(float xpos, float ypos)
            : base(xpos, ypos)
        {
            graphic = new Sprite(_rebound, 0f, 0f);
            depth = 0.9f;
            center = new Vec2(4f, 4f);
        }

        public override void Update()
        {
            alpha -= 0.07f;
            if (alpha <= 0)
                Level.Remove(this);
        }
    }
}
