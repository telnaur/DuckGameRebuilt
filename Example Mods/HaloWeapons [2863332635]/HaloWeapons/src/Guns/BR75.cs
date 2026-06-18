namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Guns)]
    [GunGameLevel(11)]
    public class BR75 : BurstingWeapon
    {
        public BR75(float x, float y) : base(x, y)
        {
            MaxBulletsFired = 3;
            BurstDelay = 7;
            FireDelay = 2f;

            ammo = 30;
            collisionSize = new Vec2(25f, 10f);
            center = collisionSize / 2f;
            collisionOffset = -center;

            _holdOffset = -new Vec2(4f, 1f);
            _barrelOffsetTL = new Vec2(29f, 6f);
            _ammoType = new ATBR75();
            _kickForce = 1f;

            _fireSound = Paths.GetSoundPath("br75Fire.wav");
        }
    }
}
