namespace DuckGame.HaloWeapons
{
    public class ATRavager : AmmoType
    {
        public ATRavager()
        {
            SetSprite(Resources.LoadSpriteMap("ravagerBullet.png", 17, 5));

            bulletThickness = 0f;
            bulletSpeed = 7f;
            range = 400f;
            accuracy = 0.9f;
            penetration = 1f;
            speedVariation = 0.5f;
            combustable = true;
            flawlessPipeTravel = true;
        }

        protected void SetSprite(SpriteMap spriteMap)
        {
            spriteMap.AddAnimation("idle", 0.3f, true, new int[]
            {
                0,
                1
            });

            spriteMap.SetAnimation("idle");
            spriteMap.CenterOrigin();

            sprite = spriteMap;
        }
    }
}
