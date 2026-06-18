namespace DuckGame.UFFMod
{
    internal class QuacktionFlash : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");
        public StateBinding _equipOwnerStateBinding = new StateBinding("_equipOwner");

        public Equipment _equipOwner;

        private SpriteMap sprite;

        public QuacktionFlash(float xpos, float ypos, Equipment e)
            : base(xpos, ypos)
        {
            sprite = new SpriteMap(Mod.GetPath<UffMod>("equipment\\quacktionReady"), 32, 32);
            sprite.AddAnimation("gurrenlagann", 0.5f, false, 7, 0, 1, 2, 3, 4, 5, 6);
            sprite.SetAnimation("gurrenlagann");
            graphic = sprite;
            center = new Vec2(16f, 16f);
            depth = 2f;
            _equipOwner = e;
        }

        public override void Update()
        {
            if (_equipOwner != null)
            {
                sprite.flipH = _equipOwner.offDir < 0;
                position = _equipOwner.position;
            }
            if (sprite.finished)
                Level.Remove(this);
        }
    }
}
