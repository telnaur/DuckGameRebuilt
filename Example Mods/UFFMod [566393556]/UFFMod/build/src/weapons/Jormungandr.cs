namespace DuckGame.UFFMod
{
    [EditorGroup("uff|weapons|tech")]
    public class Jormungandr : Gun
    {
        private SpriteMap sprite;

        public Jormungandr(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\jormungandr"), 26, 10);
            graphic = sprite;
            _center = new Vec2(13f, 5f);
            _collisionSize = new Vec2(20f, 8f);
            _collisionOffset = new Vec2(-10f, -4f);
            _holdOffset = new Vec2(5f, -1f);
            _barrelOffsetTL = new Vec2(24f, 2f);

            // weapon settings
            ammo = 10;
            _ammoType = new ATWorldEater();
            _fireSound = Mod.GetPath<UffMod>("SFX\\peow");
            _kickForce = 2f;
            _weight = 3f;
            _fireWait = 1.5f;
            _fullAuto = true;
            _flare = new SpriteMap(Mod.GetPath<UffMod>("weapons\\jormungandrFlare"), 16, 16);
            _flare.center = new Vec2(0f, 8f);
        }
    }
}
