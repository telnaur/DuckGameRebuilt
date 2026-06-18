namespace DuckGame.HaloWeapons
{
    public class MaterialPaintBucket : Material
    {
        private Color _color;

        public MaterialPaintBucket(Color color)
        {
            LoadEffect();
            Color = color;
        }

        public MaterialPaintBucket()
        {
            LoadEffect();
        }

        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                if (_color == value)
                    return;

                _color = value;
                SetValue("u_color", value);
            }
        }

        private void LoadEffect()
        {
            _effect = Resources.LoadMTEffect("paintBucket.xnb");
        }
    }
}
