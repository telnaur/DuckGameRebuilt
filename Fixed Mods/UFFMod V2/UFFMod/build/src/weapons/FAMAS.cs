namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|guns")]
    public class FAMAS : PewPewLaser
    {
        private SpriteMap sprite;

        public FAMAS(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\FAMAS"), 26, 11);
            graphic = sprite;
            _center = new Vec2(13f, 6f);
            _collisionSize = new Vec2(26f, 11f);
            _collisionOffset = new Vec2(-13f, -6f);
            _holdOffset = new Vec2(-3f, 0f);
            _barrelOffsetTL = new Vec2(25f, 4f);

            // weapon settings
            ammo = 25;
            _ammoType = new AT9mm();
            _fireSound = "deepMachineGun2";
            _kickForce = 1.2f;
            _fireWait = 2f;
            _flare = new SpriteMap("smallFlare", 11, 10, false);
            _flare.center = new Vec2(0f, 5f);
        }

        public override void Update()
        {
            base.Update();
            if (_burstWait > 0.75f)
                _burstWait = 0.75f;
        }
    }
}
