namespace DuckGame.UFFMod
{
    // [EditorGroup("uff|weapons|testing")]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isSuperWeapon", true)]
    internal class ERRNULL : Gun
    {
        private SpriteMap sprite;

        public ERRNULL(float xpos, float ypos)
          : base(xpos, ypos)
        {
            // collision & sprite settings
            sprite = new SpriteMap(Mod.GetPath<UffMod>("weapons\\errnull"), 24, 12);
            graphic = sprite;
            _center = new Vec2(12f, 6f);
            _collisionSize = new Vec2(16f, 13f);
            _collisionOffset = new Vec2(-7f, -7f);
            _holdOffset = new Vec2(6f, 0f);
            
            // weapon settings
            ammo = 1;
            _weight = 9f;
        }

        public override void Fire()
        {
            if (duck != null && ammo > 0)
            {
                SFX.Play(Mod.GetPath<UffMod>("SFX\\exception.wav"), 1f, 0, 0f, false);
                duck.resetAction = true;
                Level.Add(new NotARealErrorHandler(0f, 0f));
                ammo--;
            }
        }
    }
}