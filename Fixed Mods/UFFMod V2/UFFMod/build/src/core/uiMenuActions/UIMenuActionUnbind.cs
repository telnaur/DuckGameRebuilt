namespace DuckGame.UFFMod
{
    internal class UIMenuActionUnbind : UIMenuActionCloseMenu
    {
        Duck duck;

        public UIMenuActionUnbind(UIComponent _menu, Duck _duck)
            : base(_menu)
        {
            duck = _duck;
        }

        public override void Activate()
        {
            UffMod.Main.BindFunction(duck);

            base.Activate();
        }
    }
}
