namespace DuckGame.HaloWeapons
{
    [EditorGroup(EditorGroups.Guns)]
    [GunGameLevel(1)]
    public class MA40 : HaloWeapon
    {
        public MA40(float x, float y) : base(x, y)
        {
            ammo = 30;

            _ammoType = new ATMA40();
            _fullAuto = true;
            _barrelOffsetTL = new Vec2(31f, 4f);
            _kickForce = 2f;
            _fireWait = 1.2f;
            _fireSound = Paths.GetSoundPath("ma40Fire.wav");
        }

        public override void Fire()
        {
            base.Fire();

            if (ammo > 0)
            {
                SetAnimation(null);
                SetAnimation("fire");
            }
        }

        protected override Sprite CreateGraphics(string spritePath)
        {
            var sprite = new SpriteMap(spritePath, 30, 13);

            sprite.AddAnimation("fire", 0.9f, false, new int[]
            {
                0, 
                1, 
                0
            });

            return sprite;
        }

        private void SetAnimation(string name)
        {
            SpriteMap.SetAnimation(name);
        }
    }
}
