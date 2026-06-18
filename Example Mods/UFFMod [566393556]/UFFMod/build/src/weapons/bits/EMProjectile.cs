
namespace DuckGame.UFFMod
{
    internal class EMProjectile : Bullet
    {
        public EMProjectile(float xpos, float ypos, AmmoType type, float ang = -1f, Thing owner = null, bool rbound = false, float distance = -1f, bool tracer = false, bool network = false)
            : base(xpos, ypos, type, ang, owner, rbound, distance, tracer, network)
        {
        }

        public override void Draw()
        {
            color = new Color(225, 30, 30);
            base.Draw();
        }
    }
}
