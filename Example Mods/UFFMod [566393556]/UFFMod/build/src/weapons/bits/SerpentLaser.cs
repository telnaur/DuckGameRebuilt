namespace DuckGame.UFFMod
{
    internal class SerpentLaser : LaserBullet
    {
        private Tex2D _beem;
        private float _thickness;

        public SerpentLaser(float xpos, float ypos, AmmoType type, float ang = -1f, Thing owner = null, bool rbound = false, float distance = -1f, bool tracer = false, bool network = false)
            : base(xpos, ypos, type, ang, owner, rbound, distance, tracer, network)
        {
            _thickness = type.bulletThickness;
            _beem = Content.Load<Tex2D>(Mod.GetPath<UffMod>("weapons\\serpentBeam"));
        }

        public override void Draw() // decompiled garbage follows
        {
            if (_tracer || _bulletDistance <= 0.1f)
                return;
            float length = (drawStart - drawEnd).length;
            float val = -8f;
            float num1 = (float)(length / 8f);
            float num2 = 0f;
            float num3 = 8f;
            bool flag = false;
            while (!flag)
            {
                val += 8f;
                if (val + num3 > length)
                {
                    num3 = length - Maths.Clamp(val, 0.0f, 99f);
                    flag = true;
                }
                num2 += num1;
                Graphics.DrawTexturedLine(_beem, drawStart + travelDirNormalized * val, drawStart + travelDirNormalized * (val + num3), Color.White * num2, _thickness, 0.6f);
            }
        }

        protected override void Rebound(Vec2 pos, float dir, float rng)
        {
            isRebound = true;
            SerpentLaser serpentLaser = new SerpentLaser(pos.x, pos.y, ammo, dir, null, rebound, rng);
            isRebound = false;
            serpentLaser._teleporter = _teleporter;
            serpentLaser.firedFrom = firedFrom;
            Level.current.AddThing(serpentLaser);
            Level.current.AddThing(new SerpentRebound(pos.x, pos.y));
        }
    }
}
