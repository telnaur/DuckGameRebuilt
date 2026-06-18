namespace DuckGame.HaloWeapons
{
    public class DesertBackground : HaloBackground
    {
        public DesertBackground(float x, float y) : base(x, y)
        {
            SpriteName = "desertBackground.png";
            DefaultSpeed = 0.4f;

            backgroundColor = new Color(198, 107, 57);
            graphic = new SpriteMap("backgroundIcons", 16, 16, false)
            {
                frame = 7
            };
          
            _editorName = "Desert BG";
        }

        protected override void AddZones(ParallaxBackground parallax)
        {
            AddZonesRange(parallax, 0, 2, 0f, -DefaultSpeed, true);
            AddZonesRange(parallax, 3, 4, 0.2f, -DefaultSpeed, true);
            AddZonesRange(parallax, 5, 6, 0.4f, -DefaultSpeed, true);

            AddZonesRange(parallax, 7, 15, 0.8f);

            AddSprite(parallax, Resources.LoadSprite("planet.png"), new Vec2(193f, 59f), 12, 0.6f);

            AddZonesRange(parallax, 16, 21, 0.6f);
            AddZonesRange(parallax, 22, 30, 0.4f);
        }
    }
}
