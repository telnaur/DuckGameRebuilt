namespace DuckGame.UFFMod
{
    internal class UIMenuActionPrank : UIMenuActionCloseMenu
    {
        string prankType;
        Duck duck;

        public UIMenuActionPrank(UIComponent _menu, string _prankType, Duck _duck)
            : base(_menu)
        {
            prankType = _prankType;
            duck = _duck;
        }

        public override void Activate()
        {
            UffMod.Main.BindFunction(duck, new UffFunctionPrank(prankType));

            base.Activate();
        }
    }
}
