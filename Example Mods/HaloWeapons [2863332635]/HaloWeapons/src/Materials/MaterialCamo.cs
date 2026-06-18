namespace DuckGame.HaloWeapons
{
    public class MaterialCamo : Material
    {
        private readonly Sprite _sprite;

        private float _deltaTime;
        private float _time;

        public MaterialCamo(Sprite sprite, float deltaTime)
        {
            _effect = Resources.LoadMTEffect("camo.xnb");
            _sprite = sprite;
            _deltaTime = deltaTime;
        }

        public Vec2 CenterOffset { get; set; }

        public override void Update()
        {
            _time += _deltaTime;
        }

        public override void Apply()
        {
            SetValue("u_time", _time);

            Tex2D texture = _sprite.texture;

            SetValue("u_textureRes", new Vec2(texture.width, texture.height));
            SetValue("u_frameRes", new Vec2(texture.frameWidth, texture.frameHeight));
            SetValue("u_offset", CenterOffset);

            base.Apply();
        }

        public void StartDeactivating()
        {
            _time = 0.5f;
            _deltaTime *= -1f;
        }
    }
}
