namespace DuckGame.UFFMod
{
    public class GlobalExplosion : Thing
    {
        public StateBinding _positionStateBinding = new CompressedVec2Binding("position");

        private SpriteMap _sprite;
        private int _smokeFrame;
        private bool _smoked;

        public GlobalExplosion(float xpos, float ypos)
            : base(xpos, ypos)
        {
            _sprite = new SpriteMap("explosion", 64, 64);
            _sprite.AddAnimation("explode", 1f, false, 0, 0, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            _sprite.SetAnimation("explode");
            graphic = _sprite;
            _sprite.speed = 0.4f + Rando.Float(0.2f);
            xscale = 0.5f + Rando.Float(0.5f);
            yscale = xscale;
            center = new Vec2(32f, 32f);
            _smokeFrame = Rando.Int(1, 3);
            depth = 1f;
            vSpeed = Rando.Float(-0.2f, -0.4f);
        }

        public override void Update()
        {
            if (_sprite.frame > _smokeFrame && !_smoked)
            {
                int num = Graphics.effectsLevel == 2 ? Rando.Int(1, 4) : 1;
                for (int index = 0; index < num; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(x + Rando.Float(-5f, 5f), y + Rando.Float(-5f, 5f));
                    smallSmoke.vSpeed = Rando.Float(0.0f, -0.5f);
                    smallSmoke.xscale = smallSmoke.yscale = Rando.Float(0.2f, 0.7f);
                    Level.Add((Thing)smallSmoke);
                }
                _smoked = true;
            }
            y += (float)(double)vSpeed;
            if (!_sprite.finished)
                return;
            Level.Remove(this);
        }
    }
}
