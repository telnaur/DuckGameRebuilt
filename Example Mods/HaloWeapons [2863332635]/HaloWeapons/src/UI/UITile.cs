using System;
using System.Text;

namespace DuckGame.HaloWeapons
{
    public class UITile
    {
        private readonly BitmapFont _font = new BitmapFont("biosFont", 8)
        {
            scale = new Vec2(0.4f)
        };

        private EventHandler _click;

        private float _textScrollTimer;
        private float _textScrollPauseTimer;
        private int _textScrollIndex;

        private float _blinkTimer;

        private string _text;

        public event EventHandler Click
        {
            add
            {
                _click += value;
            }

            remove
            {
                _click -= value;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                if (_text == value)
                    return;

                _text = value;

                _textScrollTimer = 0;
                _textScrollPauseTimer = 0;
            }
        }

        public Vec2 Position { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float TextScrollInterval { get; set; } = 0.2f;
        public float TextScrollPauseTime { get; set; } = 1f;
        public int TextScrollGap { get; set; } = 3;
        public Color Color { get; set; } = Color.LightGray;
        public Color BorderColor { get; set; } = UI.MenuItemColor;
        public Sprite Sprite { get; set; }
        public bool Enabled { get; set; } = true;

        protected BitmapFont Font => _font;

        public virtual void Draw()
        {
            var rectangle = new Rectangle(Position, Position + new Vec2(Width, Height));

            Graphics.DrawRect(rectangle, Color, 2.9f);
            Graphics.DrawRect(rectangle, BorderColor, 2.9f, false);

            float centerX = Position.x + Width / 2;

            if (Sprite is not null)
                Graphics.Draw(Sprite, centerX - Sprite.width / 2f, Position.y + Height / 2f - Sprite.height / 2f, 3f);

            string text = GetDisplayedText();

            if (!string.IsNullOrEmpty(Text))
                _font.Draw(text, new Vec2(centerX - _font.GetWidth(text) / 2f + 0.25f, Position.y + Height - _font.height - 3f), Color.Black, 3f);

            if (!Enabled)
            {
                Graphics.DrawRect(rectangle, Color.Gray * 0.5f, 3f);
            }
            else if (_blinkTimer > 0f)
            {
                Graphics.DrawRect(rectangle, Color.White * 0.7f, 3f);

                _blinkTimer -= 0.01f;
            }
        }

        public void OnClick()
        {
            if (!Enabled)
                return;

            _blinkTimer = 0.04f;

            _click?.Invoke(this, EventArgs.Empty);
        }

        private string GetDisplayedText()
        {
            if (TextFits(_text))
                return _text;

            int length = _text.Length;

            if (_textScrollPauseTimer <= 0f)
            {
                if (_textScrollTimer <= 0f)
                {
                    _textScrollIndex += 1;

                    if (_textScrollIndex >= length + TextScrollGap)
                    {
                        _textScrollIndex = 0;
                        _textScrollPauseTimer = TextScrollPauseTime;
                    }

                    _textScrollTimer = TextScrollInterval;
                }
                else
                {
                    _textScrollTimer -= 0.01f;
                }
            }
            else
            {
                _textScrollPauseTimer -= 0.01f;
            }

            var builder = new StringBuilder();
            int index = _textScrollIndex;

            while (TextFits(builder.ToString()))
            {
                if (index >= length)
                {
                    int difference = index - length;

                    if (difference < TextScrollGap)
                        builder.Append(" ");
                    else
                        builder.Append(_text[difference - TextScrollGap]);
                }
                else
                {
                    builder.Append(_text[index]);
                }

                index++;
            }

            return builder.ToString();  
        }

        private bool TextFits(string text)
        {
            return _font.GetWidth(text) <= Width - 4.5f;
        }
    }
}
