namespace DuckGame.HaloWeapons
{
    public class UISkinTile : UITile
    {
        public Sprite UpperSprite { get; set; }
        public Color CostColor { get; set; } = Color.Black;
        public int? Cost { get; set; }

        public override void Draw()
        {
            if (UpperSprite is not null)
                Graphics.Draw(UpperSprite, Position.x + 2f, Position.y + 2f, 3f);

            if (Cost is not null)
            {
                string cost = $"{Cost.Value}$";
                Font.Draw(cost, new Vec2(Position.x + Width - Font.GetWidth(cost) - 2f, Position.y + Font.height), CostColor, 3f);
            }

            base.Draw();
        }
    }
}
