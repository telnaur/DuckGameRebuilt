namespace DuckGame.HaloWeapons
{
    public abstract class FadingParticle : PhysicsParticle
    {
        public FadingParticle(float x, float y) : base(x, y)
        {

        }

        public virtual Color Color { get; set; }

        public override void Update()
        {
            alpha -= 0.01f;

            if (alpha <= 0f)
                Level.Remove(this);

            base.Update();
        }
    }
}
