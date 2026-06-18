using System;

namespace DuckGame.UFFMod
{
    internal class UIMenuActionGiveItem : UIMenuActionCloseMenu
    {
        Type itemType;
        Duck duck;

        public UIMenuActionGiveItem(UIComponent _menu, Type _itemType, Duck _duck)
            : base(_menu)
        {
            itemType = _itemType;
            duck = _duck;
        }

        public override void Activate()
        {
            UffMod.Main.BindFunction(duck, new UffFunctionGive(itemType));

            base.Activate();
        }
    }
}
