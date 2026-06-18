namespace DuckGame.UFFMod
{
    internal class AmmoBoxReload : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        private SpriteMap sprite;
        private int timer;

        public AmmoBoxReload(float xpos, float ypos)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("stuff\\blocks\\reload"), 28, 7);
            sprite.AddAnimation("reload", 0.25f, true, 0, 1);
            sprite.SetAnimation("reload");
            graphic = sprite;
            center = new Vec2(14f, 4f);
            xscale = yscale = 1.5f;
            depth = 1f;
            vSpeed = -0.8f;
            sprite.CenterOrigin();
        }

        public override void Update()
        {
            y += vSpeed;
            vSpeed = MathHelper.Lerp(vSpeed, 0f, 0.04f);
            timer++;
            if (timer > 30)
                Level.Remove(this);
        }
    }
}
