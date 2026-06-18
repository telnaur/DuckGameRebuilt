namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Guns)]
    [GunGameLevel(12)]
    public class Cindershot : HaloWeapon
    {
        public Cindershot(float x, float y) : base(x, y)
        {
            ammo = 10;

            _flare = Resources.LoadSpriteMap("cindershotFlare.png", 13, 9);
            _flare.center = new Vec2(-1f, _flare.height / 2f);
            
            _ammoType = new ATCindershot();
            _holdOffset = new Vec2(-3f, 0f);
            _bulletColor = _ammoType.bulletColor;
            _barrelOffsetTL = new Vec2(28f, 4f);
            _fireSound = Paths.GetSoundPath("cindershotFire.wav");
            _fireWait = 3f;
            _kickForce = 5f;
        }

        public override void Fire()
        {
            bool canShoot = ammo > 0 && _wait <= 0f;

            base.Fire();

            if (canShoot)
                _flareAlpha = 2.5f;
        }
    }
}
