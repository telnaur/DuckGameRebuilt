using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.HaloWeapons
{
    public static class UI
    {
        public static readonly Color MenuItemColor = Lerp.Color(Color.LightGray, Color.Black, 0.7f);

        private const string ControlButtons = "@CANCEL@BACK @SELECT@SELECT";

        private static readonly List<UIComponent> s_addInNextFrame = new List<UIComponent>();
        private static readonly Vec2 s_creditDisplayPosition = new Vec2(270f, 20f);

        private static UIComponent s_currentOptionsGroup;
        private static UIMenuGray s_skinsShopMenu;
        private static CreditDisplayUIComponent s_creditDisplay;

        public static void Update()
        {
            if (s_currentOptionsGroup is not null)
            {
                foreach (UIComponent component in s_addInNextFrame)
                {
                    s_currentOptionsGroup.Add(component, false);
                    component.Open();
                }

                if (s_skinsShopMenu is not null && s_skinsShopMenu.open)
                {
                    if (!s_currentOptionsGroup.components.Contains(s_creditDisplay))
                    {
                        s_creditDisplay = new CreditDisplayUIComponent(new CreditDisplay(s_creditDisplayPosition.x, s_creditDisplayPosition.y));
                        s_currentOptionsGroup.Add(s_creditDisplay, false);
                        s_creditDisplay.Open();
                    }
                }
                else if (s_creditDisplay is not null)
                {
                    s_creditDisplay.Close();
                    s_creditDisplay = null;
                }
            }

            s_addInNextFrame.Clear();
        }

        public static void AddSkinsItemToPauseMenu(UIBox section, UIMenu pauseMenu, UIComponent pauseGroup)
        {
            AddSkinsItemToMenu(section, pauseMenu, pauseGroup);
        }

        public static void AdSkinsItemToDucknetMenu()
        {
            DuckNetworkCore core = DuckNetwork.core;
            UIMenu parent = core._ducknetMenu;

            AddSkinsItemToMenu(parent, parent, core.ducknetUIGroup);

            if (Network.isClient)
                AddEmptyLine(parent);
        }

        public static void AddCreditIncreaseDisplay(int initialCredits)
        {
            if (Level.current is null)
                return;

            Level.Add(new CreditIncreaseDisplay(initialCredits, s_creditDisplayPosition.x, s_creditDisplayPosition.y));
        }

        private static void AddSkinsItemToMenu(UIBox addTo, UIMenu parent, UIComponent optionsGroup)
        {
            UIMenuGray skinsMenu = BuildSkinsMenu(parent, optionsGroup);

            AddEmptyLine(addTo);
            addTo.Add(new UIMenuItem("SKINS", new UIMenuActionOpenMenu(parent, skinsMenu), c: new Color(173, 173, 173)));
        }

        private static UIMenuGray BuildSkinsMenu(UIMenu parent, UIComponent optionsGroup)
        {
            var menu = CreateSmallMenu("SKINS");

            UIMenuGray weaponsMenu = BuildWeaponsMenu(menu, optionsGroup, (type, uiMenu) =>
            {
                return BuildSkinsShopMenu(type, uiMenu);
            });
            UIMenuGray inverntoryMenu = BuildWeaponsMenu(menu, optionsGroup, (type, uiMenu) =>
            {
                return BuildSkinsInventoryMenu(type, uiMenu);
            });


            menu.Add(new UIMenuItem("BUY", new UIMenuActionOpenMenu(menu, weaponsMenu), c: MenuItemColor));
            menu.Add(new UIMenuItem("EQIUP", new UIMenuActionOpenMenu(menu, inverntoryMenu), c: MenuItemColor));

            AddEmptyLine(menu);

            UIMenuGray optionsMenu = BuildOptionsMenu(menu, optionsGroup);
            menu.Add(new UIMenuItem("OPTIONS", new UIMenuActionOpenMenu(menu, optionsMenu), c: MenuItemColor));

            AddEmptyLine(menu);

            var backFunction = new UIMenuActionOpenMenu(menu, parent);

            menu.Add(new UIMenuItem("BACK", backFunction, c: MenuItemColor));
            menu.SetBackFunction(backFunction);

            InitializeMenu(menu, optionsGroup);

            return menu;
        }

        private static UIMenuGray BuildWeaponsMenu(UIMenu parent, UIComponent optionsGroup, Func<Type, UIMenu, UIMenu> createSkinsMenu)
        {
            var menu = CreateBigMenu("SELECT WEAPON");

            List<UITile> tiles = new List<UITile>();

            foreach (Type type in Skins.GetSkinWeaponTypes())
            {
                var tile = new UITile()
                {
                    Text = type.Name,
                    Sprite = Skins.CreateDemoSprite(type, Skins.GetDefaultSkinPath(type))
                };

                tile.Click += (sender, eventArgs) =>
                {
                    var skinShopMenu = createSkinsMenu(type, menu);

                    menu.Close();

                    if (MonoMain.pauseMenu == menu)
                        MonoMain.pauseMenu = skinShopMenu;
                };

                tiles.Add(tile);
            }

            menu.Add(new UITileMenu(tiles, menu.width - 16f, 100f));
            menu.SetBackFunction(new UIMenuActionOpenMenu(menu, parent));

            InitializeMenu(menu, optionsGroup);

            return menu;
        }

        private static UIMenuGray BuildSkinsShopMenu(Type weaponType, UIMenu parent)
        {
            var menu = CreateBigMenu("SHOP");
            var tiles = new List<UISkinTile>();

            foreach (Skin skin in Skins.GetAll(weaponType))
            {
                int cost = skin.Cost;
                bool hasSkin = Skins.HasSkin(weaponType, skin.Index);

                var tile = new UISkinTile()
                {
                    Text = skin.Name,
                    Sprite = Skins.CreateDemoSprite(weaponType, Skins.GetSkinPath(weaponType, skin.FileName)),
                    Enabled = !hasSkin,
                    Cost = hasSkin ? null : cost,
                    CostColor = cost <= Skins.Credits ? Color.ForestGreen : Color.Red,
                    UpperSprite = hasSkin ? CreateShoppingCartSprite() : null
                };

                tile.Click += (sender, eventArgs) =>
                {
                    if (Skins.BuySkin(weaponType, skin))
                    {
                        tile.Enabled = false;
                        tile.UpperSprite = CreateShoppingCartSprite();
                        tile.Cost = null;

                        s_addInNextFrame.Add(new CreditDisplayUIComponent(new CreditFlyAwayDisplay(cost, s_creditDisplayPosition.x, s_creditDisplayPosition.y)));

                        foreach (UISkinTile skinTile in tiles)
                            if (skinTile.Cost > Skins.Credits)
                                skinTile.CostColor = Color.Red;

                        SFX.Play(Paths.GetSoundPath("purchase.wav"), 0.6f);
                    }
                };

                tiles.Add(tile);
            }

            menu.Add(new UITileMenu(tiles, menu.width - 16f, 100f));
            menu.SetBackFunction(new UIMenuActionOpenMenu(menu, parent));

            s_skinsShopMenu = menu;
            s_addInNextFrame.Add(menu);

            return menu;
        }

        private static UIMenuGray BuildSkinsInventoryMenu(Type weaponType, UIMenu parent)
        {
            var menu = CreateBigMenu("INVENTORY");
            var tiles = new List<UITile>();

            foreach (Skin skin in Skins.GetAll(weaponType).Where(skin => Skins.HasSkin(weaponType, skin.Index)))
            {
                int index = skin.Index;

                var tile = new UISkinTile()
                {
                    Text = skin.Name,
                    Sprite = Skins.CreateDemoSprite(weaponType, Skins.GetSkinPath(weaponType, skin.FileName)),
                    UpperSprite = Skins.IsEquipped(weaponType, index) ? CreateCheckMarkSprite() : null
                };

                tile.Click += (sender, eventArgs) =>
                {
                    if (Skins.IsEquipped(weaponType, index))
                    {
                        Skins.Unequip(weaponType);
                        tile.UpperSprite = null;
                    }
                    else
                    {
                        Skins.Equip(weaponType, index);
                        tile.UpperSprite = CreateCheckMarkSprite();

                        foreach (UISkinTile skinTile in tiles)
                            if (skinTile != tile)
                                skinTile.UpperSprite = null;
                    }
                };

                tiles.Add(tile);
            }

            menu.Add(new UITileMenu(tiles, menu.width - 16f, 100f));
            menu.SetBackFunction(new UIMenuActionOpenMenu(menu, parent));

            s_addInNextFrame.Add(menu);

            return menu;
        }

        private static UIMenuGray BuildOptionsMenu(UIMenu parent, UIComponent optionsGroup)
        {
            var menu = CreateSmallMenu("OPTIONS");
            OptionsData data = Options.Data;
             
            menu.Add(new UIMenuItemToggle("SHARE", field: new FieldBinding(data, nameof(OptionsData.ShareSkins)), c: MenuItemColor));
            menu.Add(new UIMenuItemToggle("IGNORE     ", field: new FieldBinding(data, nameof(OptionsData.IgnoreOnlineSkins)), c: MenuItemColor));

            AddEmptyLine(menu);

            var backFunction = new UIMenuActionOpenMenu(menu, parent);

            menu.Add(new UIMenuItem("BACK", backFunction, c: MenuItemColor));

            InitializeMenu(menu, optionsGroup);

            return menu;
        }

        private static UIMenuGray CreateBigMenu(string title)
        {
            return new UIMenuGray(title, Layer.HUD.width / 2f, Layer.HUD.height / 2f, 180f, controlButtons: ControlButtons);
        }

        private static UIMenuGray CreateSmallMenu(string title)
        {
            return new UIMenuGray(title, 160f, 90f, 160f, controlButtons: ControlButtons);
        }

        private static void AddEmptyLine(UIBox addTo)
        {
            addTo.Add(new UIText(string.Empty, default(Color)));
        }

        private static void InitializeMenu(UIMenu menu, UIComponent optionsGroup)
        {
            menu.Update();
            optionsGroup.Add(menu, false);
            s_currentOptionsGroup = optionsGroup;
        }

        private static SpriteMap CreateCheckMarkSprite()
        {
            return CreateLabelSprite(0);
        }

        private static SpriteMap CreateShoppingCartSprite()
        {
            return CreateLabelSprite(1);
        }

        private static SpriteMap CreateLabelSprite(int frame)
        {
            return new SpriteMap(Paths.GetSpritePath("marks.png"), 8, 8)
            {
                frame = frame
            };
        }
    }
}
