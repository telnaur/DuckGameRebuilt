namespace DuckGame.HaloWeapons
{
    public class BeamHitParticle : PhysicsParticle
    {
        private readonly Color _color;
        private readonly float _maxLife;
        private readonly float _size;

        public BeamHitParticle(float x, float y, float angle) : base(x, y)
        {
            depth = 0.9f;
            velocity = new Vec2(Rando.Float(0.5f, 1f), Rando.Float(-1f, 1f)).Rotate(angle, Vec2.Zero);

            _bounceEfficiency = 0f;
            _maxLife = Rando.Float(0.1f, 0.2f);
            _color = Color.Lerp(new Color(2, 136, 209), new Color(79, 195, 247), Rando.Float(1f));
            _size = Rando.Float(1f, 2f);
            _life = _maxLife;
            _gravMult = 0f;
        }

        public override void Update()
        {
            base.Update();

            alpha = _life / _maxLife;
        }

        public override void Draw()
        {
            Graphics.DrawRect(new Rectangle(position, position + new Vec2(_size)), _color * alpha, depth);

            base.Draw();
        }
    }
}
