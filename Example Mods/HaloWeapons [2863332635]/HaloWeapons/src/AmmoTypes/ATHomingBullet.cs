namespace DuckGame.HaloWeapons
{
    public class ATHomingBullet : AmmoType
    {
        public ATHomingBullet()
        {
            accuracy = 1f;
            range = 400f;
            bulletSpeed = 8f;
            speedVariation = 0f;
            penetration = 1f;
            sprite = Resources.LoadSprite("needlerBullet", true);
            bulletThickness = 1f;
            affectedByGravity = true;
            gravityMultiplier = 0f;
            flawlessPipeTravel = true;
            accuracy = 0.3f;
            bulletType = typeof(HomingBullet);
            bulletColor = new Color(248, 186, 251);
            combustable = true;
            flawlessPipeTravel = true;
        }
    }
}
