using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace DuckGame.HaloWeapons
{
    public class UIMenuGray : UIMenu
    {
        private bool _addedThisFrame = true;

        public UIMenuGray(string title, float x, float ypos, float width = -1, float height = -1, string controlButtons = "", InputProfile profile = null, bool tiny = false) : base(title, x, ypos, width, height, controlButtons, profile, tiny)
        {
            AccessTools.Field(typeof(UIBox), "_sections").SetValue(this, new SpriteMap(Paths.GetSpritePath("uiBox.png"), 10, 10));

            ChangeTextColor(_splitter.topSection);
            ChangeTextColor(_splitter.bottomSection);
        }

        public override void Draw()
        {
            if (_addedThisFrame)
            {
                _addedThisFrame = false;
                return;
            }

            base.Draw();
        }

        private void ChangeTextColor(UIComponent parent)
        {
            FieldInfo field = AccessTools.Field(typeof(UIText), "_color");

            foreach (UIText component in parent.components.OfType<UIText>())
                field.SetValue(component, Color.Black);
                
        }
    }
}
