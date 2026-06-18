namespace DuckGame.UFFMod
{
    internal class ATWandStar : AmmoType
    {
        private SpriteMap spriteMap;

        public ATWandStar()
        {
            accuracy = 1f;
            range = 1000f;
            penetration = 1f;
            bulletSpeed = 4.2f;
            bulletThickness = 3f;
            spriteMap = new SpriteMap(Mod.GetPath<UffMod>("weapons\\wandStar"), 15, 15);
            spriteMap.AddAnimation("bapun", 0.5f, true, 0, 1, 2);
            spriteMap.SetAnimation("bapun");
            sprite = spriteMap;
            sprite.CenterOrigin();
        }
    }
}
