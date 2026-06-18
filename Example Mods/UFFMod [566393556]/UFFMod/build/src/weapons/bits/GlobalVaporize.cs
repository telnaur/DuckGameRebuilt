namespace DuckGame.UFFMod
{
    public class GlobalVaporize : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        private SpriteMap _sprite;
        private bool created;

        public GlobalVaporize(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\vaporize"), 22, 22, false);
            _sprite.AddAnimation("vaporize", 0.3f + Rando.Float(0.1f), false, 0, 1, 2, 3, 4);
            _sprite.SetAnimation("vaporize");
            graphic = _sprite;
            center = new Vec2(11f, 11f);
            depth = 1f;
            created = false;
        }

        public override void Update()
        {
            if (!created)
            {
                SFX.Play(Mod.GetPath<UffMod>("SFX\\pewpew"), 1f);
                created = true;
            }
            if (_sprite.finished)
                Level.Remove(this);
        }
    }
}
