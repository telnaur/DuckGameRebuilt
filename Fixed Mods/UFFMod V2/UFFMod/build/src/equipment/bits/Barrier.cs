namespace DuckGame.UFFMod
{
    public class Barrier : Equipment
    {
        protected SpriteMap sprite;
        protected Sprite pickupSprite;
        protected Vec2 wearCenter;

        public Barrier(float xpos, float ypos)
            : base(xpos, ypos)
        {
        }

        public override void Update()
        {
            if (equippedDuck == null)
            {
                graphic = pickupSprite;
                center = new Vec2(6f, 5f);
            }
            else
            {
                if (sprite.frame <= 5)
                    _equippedDepth = 12;
                else
                    _equippedDepth = -1;
                graphic = sprite;
                center = wearCenter;
            }

            base.Update();
        }
    }
}
