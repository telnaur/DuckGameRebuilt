namespace DuckGame.HaloWeapons
{
    public sealed class CreditDisplayUIComponent : UIComponent
    {
        private readonly CreditDisplay _display;

        public CreditDisplayUIComponent(CreditDisplay display) : base(0f, 0f, 0f, 0f)
        {
            _display = display; 
        }

        public override void Update()
        {
            if (_close)
                return;

            _display.Update();
        }

        public override void Draw()
        {
            if (_close)
                return;

            _display.Draw();
        }
    }
}
