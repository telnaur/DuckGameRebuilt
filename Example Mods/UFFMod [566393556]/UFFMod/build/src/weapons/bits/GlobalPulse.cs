namespace DuckGame.UFFMod
{
    public class GlobalPulse : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        protected SpriteMap _sprite;

        public GlobalPulse(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\impulseShockwave"), 160, 160);
            _sprite.AddAnimation("impulse", 0.75f, false, 0, 1, 2, 3, 4, 5);
            _sprite.SetAnimation("impulse");
            graphic = _sprite;
            center = new Vec2(80f, 80f);
            depth = 1f;
        }

        public override void Update()
        {
            if (!_sprite.finished)
                return;
            Level.Remove(this);
        }
    }
}
