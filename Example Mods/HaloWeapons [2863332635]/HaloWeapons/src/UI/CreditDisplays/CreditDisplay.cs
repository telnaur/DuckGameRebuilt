using System;

namespace DuckGame.HaloWeapons
{
    public class CreditDisplay : Thing
    {
        private readonly Sprite _sprite = new Sprite(Paths.GetSpritePath("credit.png"));
        private readonly FancyBitmapFont _font = new FancyBitmapFont("smallFont");
        private readonly float _destinationY;

        public CreditDisplay(float x, float y) : base(x, y)
        {
            position = new Vec2(x, -_sprite.height);
            layer = Layer.HUD;
            depth = 3f;

            _destinationY = y;
        }

        public bool DrawCoin { get; set; } = true;
        protected virtual int DisplayedValue => Skins.Credits;
        protected FancyBitmapFont Font => _font;

        public override void Update()
        {
            y = Lerp.Float(y, _destinationY, Math.Abs(_destinationY - y) / 8f);
        }

        public override void Draw()
        {
            if (DrawCoin)
                Graphics.Draw(_sprite, x, y, 3f);

            string displayedString = $"{DisplayedValue}$";

            Vec2 scale = _font.scale;
            Font.scale = new Vec2(1f);

            float fullWidth = _font.GetWidth(displayedString);

            Font.scale = scale;

            float width = _font.GetWidth(displayedString);  

            _font.DrawOutline($"{DisplayedValue}$", new Vec2(x + _sprite.width + (fullWidth - width) / 2f + 1f, y + (_sprite.height - _font.characterHeight) / 2f), Color.White * alpha, Color.Black * alpha, depth);
        }
    }
}
