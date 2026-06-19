using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame.UFFMod
{
    internal class UffMain : IAutoUpdate
    {
        public const string PrankGodmode = "PRANK: GODMODE";
        public const string PrankRoast = "PRANK: ROAST";
        public const string PrankWaltz = "PRANK: WALTZ";
        public const string PrankTrollbombs = "PRANK: TROLLBOMBS";
        public const string PrankIce = "PRANK: ICE";
        public const string PrankGolden = "PRANK: GOLDEN";

        private IDictionary<Profile, UffFunction> boundFunctions;
        private IDictionary<TeamHat, Cape> teamSpawnsDone;

        public UffMain()
        {
            boundFunctions = new Dictionary<Profile, UffFunction>();
            teamSpawnsDone = new Dictionary<TeamHat, Cape>();
            DevString = "WONDER WHAT THIS COULD BE";

            AutoUpdatables.Add(this);
        }

        public string DevString { get; set; }

        public void Update()
        {
            if (Level.current == null
                || UffMod.UffDevelopers == null
                || Steam.user == null)
                return;

            if (Level.current is Editor)
            {
                // opens the editor dev console
                InputProfile inputProfile = InputProfile.Get(InputProfile.MPPlayer1);
                if ((Array.Exists(UffMod.UffDevelopers, e => e == Steam.user.id)
                    || UffMod.AssemblyName == "UFFMod-dev")
                    && inputProfile != null
                    && inputProfile.Down(Triggers.RightTrigger)
                    && inputProfile.Down(Triggers.LeftTrigger))
                    OpenUFFConsole();
            }
            else
            {
                foreach (Duck duck in Level.current.things[typeof(Duck)])
                {
                    bool isDev = duck.IsDev();

                    // handles dev commands
                    if (isDev
                        && duck.inputProfile.Down(Triggers.RightTrigger)
                        && MonoMain.pauseMenu == null)
                    {
                        if (duck.inputProfile.Pressed(Triggers.Quack) && boundFunctions != null && boundFunctions.ContainsKey(duck.profile))
                        {
                            UffFunction function;
                            if (boundFunctions.TryGetValue(duck.profile, out function))
                                function.Call(duck);
                        }
                        if (duck.inputProfile.Down(Triggers.LeftTrigger))
                            OpenUFFConsole(duck);
                    }
                }

                IEnumerable<Thing> teamHatList = Level.current.things[typeof(TeamHat)];
                foreach (TeamHat teamHat in teamHatList)
                    if (teamHat.team != null && teamHat.team.customData == null && teamHat.team.hasHat)
                    {
                        // bool isLobby = Level.current is TeamSelect2;

                        // does things when hats spawn (e.g. add capes, change quack SFX)
                        if (teamHat.team.hat.texture.textureName == Mod.GetPath<UffMod>("hats\\kero"))
                            if (teamHat is KeroHat)
                                SpawnCape(teamHat, new Sprite(Mod.GetPath<UffMod>("capes\\kero" + Rando.Int(1, 3))).texture);
                            else
                                ReplaceHat(teamHat, new KeroHat(teamHat.x, teamHat.y, teamHat.team));
                        else if (teamHat.team.hat.texture.textureName == Mod.GetPath<UffMod>("hats\\shadowSpectre"))
                            SpawnCape(teamHat, new Sprite(Mod.GetPath<UffMod>("capes\\shadowSpectre")).texture);
                        else if (teamHat.team.hat.texture.textureName == Mod.GetPath<UffMod>("hats\\alex"))
                            SpawnCape(teamHat, new Sprite(Mod.GetPath<UffMod>("capes\\alex")).texture);
                        else if (teamHat.team.hat.texture.textureName == Mod.GetPath<UffMod>("hats\\garo"))
                            if (teamHat is GaroHat)
                                SpawnCape(teamHat, new Sprite(Mod.GetPath<UffMod>("capes\\garo")).texture);
                            else
                                ReplaceHat(teamHat, new GaroHat(teamHat.x, teamHat.y, teamHat.team));
                        else if (teamHat.team.hat.texture.textureName == Mod.GetPath<UffMod>("hats\\pure"))
                            SpawnCape(teamHat, new Sprite(Mod.GetPath<UffMod>("capes\\pure")).texture);
                        else if (teamHat.team.hat.texture.textureName == Mod.GetPath<UffMod>("hats\\spriter")
                            && !(teamHat is YeeHat))
                            ReplaceHat(teamHat, new YeeHat(teamHat.x, teamHat.y, teamHat.team));
                        else if (teamHat.team.hat.texture.textureName == Mod.GetPath<UffMod>("hats\\aHat")
                           && !(teamHat is AHat))
                            ReplaceHat(teamHat, new AHat(teamHat.x, teamHat.y, teamHat.team));
                    }

                List<Thing> removedHats = teamSpawnsDone.Keys.Except(Level.current.things[typeof(TeamHat)]).ToList();
                foreach (Thing thing in removedHats)
                {
                    TeamHat teamHat = thing as TeamHat;
                    if (teamHat != null && teamSpawnsDone.ContainsKey(teamHat))
                    {
                        Cape cape;
                        if (teamSpawnsDone.TryGetValue(teamHat, out cape))
                            Level.Remove(cape);
                        teamSpawnsDone.Remove(teamHat);
                    }
                }
            }
        }

        private void ReplaceHat(TeamHat teamHat, TeamHat newHat)
        {
            if (teamHat == null
                || teamSpawnsDone.ContainsKey(teamHat)
                || Level.current == null
                || !(Level.current is GameLevel
                || Level.current is Editor
                || Level.current is TeamSelect2))
                return;

            if (teamHat.isServerForObject)
            {
                Level.Add(newHat);
                Duck d = teamHat.equippedDuck;
                if (d != null)
                {
                    d.Equip(newHat, false);
                    d.Fondle(newHat);

                    TeamSelect2 lobby = Level.current as TeamSelect2;
                    if (lobby != null)
                    {
                        ProfileBox2 box = lobby.GetBox(d.PlayerIndex());
                        box._hatSelector.hat = newHat;
                    }
                }
            }
            Level.Remove(teamHat);
            teamSpawnsDone.Add(teamHat, null);
        }

        private void SpawnCape(TeamHat teamHat, Tex2D capeTexture, bool glitch = false)
        {
            if (teamHat == null
                || teamSpawnsDone.ContainsKey(teamHat)
                || Level.current == null
                || !(Level.current is GameLevel
                || Level.current is Editor
                || Level.current is TeamSelect2))
                return;

            Cape cape = new Cape(teamHat.x, teamHat.y, teamHat);
            cape._capeTexture = capeTexture;
            Level.Add(cape);
            teamSpawnsDone.Add(teamHat, cape);
        }

        internal void BindFunction(Duck duck, UffFunction function = null)
        {
            if (boundFunctions.ContainsKey(duck.profile))
                boundFunctions.Remove(duck.profile);

            if (function != null)
            {
                KeyValuePair<Profile, UffFunction> kvp = new KeyValuePair<Profile, UffFunction>(duck.profile, function);
                boundFunctions.Add(kvp);
            }
        }

        private void OpenUFFConsole(Duck duck = null)
        {
            UIComponent uiGroup = new UIComponent(160f, 90f, 0, 0);
            UIMenu uiMenu = new UIMenu("@LWING@UFFMOD DEVELOPER CONSOLE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f, conString: "@SELECT@SELECT @QUACK@BACK");
            BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
            List<UIComponent> uiList = new List<UIComponent>() { 
                new UIText("", Color.LightGray),
                new UIText(DevString, Color.LightGray),
                new UIText("", Color.LightGray),
                // new UIMenuItemSlider("|LIME|GRAVITY MODIFIER", field: new FieldBinding(gravityAdjuster, "gravity", 0f, 1f, 0.1f), step: 0.125f),
                // new UIMenuItemToggle("|LIME|GRAVITY TOGGLE", field: new FieldBinding(gravityAdjuster, "gravityToggle", 0f, 1f, 0.1f)),
                new UIMenuItem("EXIT", new UIMenuActionCloseMenu(uiGroup), backButton: true) };
            if (duck != null)
            {
                List<UIComponent> uiListDuck = new List<UIComponent>() { new UIText("CHOOSE AN ITEM TO BIND TO RT+Q,", Color.LightGray),
                    new UIText("OR ADJUST SOME RAD SLIDERS, YO.", Color.LightGray),
                    new UIText("", Color.LightGray),
                    new UIMenuItem("GIVE GRAVITY GUN", new UIMenuActionGiveItem(uiGroup, typeof(GravityGun), duck)),
                    new UIMenuItem("GIVE PORTABLE TELEPORTER", new UIMenuActionGiveItem(uiGroup, typeof(PortableTeleporter), duck)),
                    new UIMenuItem("|DGRED|C|ORANGE|R|YELLOW|E|GREEN|A|DGBLUE|T|PURPLE|E |PINK|T|DGRED|H|ORANGE|E |YELLOW|H|GREEN|E|DGBLUE|A|PURPLE|V|PINK|E|DGRED|N|ORANGE|S", new UIMenuActionGiveItem(uiGroup, typeof(DORILLU), duck)),
                    new UIMenuItem("|WHITE|ASCEND", new UIMenuActionPrank(uiGroup, PrankGodmode, duck)),
                    new UIMenuItem("|DARKNESS|WALTZ OF THE WEAPONS", new UIMenuActionPrank(uiGroup, PrankWaltz, duck)),
                    new UIMenuItem("|DGRED|DO NOT PRESS", new UIMenuActionPrank(uiGroup, PrankTrollbombs, duck)),
                    new UIMenuItem("|PINK|ROAST 'EM", new UIMenuActionPrank(uiGroup, PrankRoast, duck)),
                    new UIMenuItem("|DGBLUE|CHILL OUT", new UIMenuActionPrank(uiGroup, PrankIce, duck)),
                    new UIMenuItem("|YELLOW|BLINGIFY", new UIMenuActionPrank(uiGroup, PrankGolden, duck)),
                    new UIMenuItem("|PURPLE|UNBIND CURRENT COMMAND", new UIMenuActionUnbind(uiGroup, duck)) };
                foreach (UIComponent uiComponent in uiListDuck)
                    uiList.Insert(uiList.Count - 1, uiComponent);
            }
            if (Level.current is Editor)
                uiList.Insert(uiList.Count - 1, new UIMenuItem("|GREEN|CONVERT DEV MAP", new UIMenuActionConvertMap(uiGroup)));
            foreach (UIComponent uiComponent in uiList)
            {
                UIText uiText = uiComponent as UIText;
                if (uiText != null)
                    uiText.SetFont(f);
                uiMenu.Add(uiComponent);
            }
            uiGroup.Add(uiMenu, false);
            Level.Add(uiGroup);
            uiGroup.Open();
            uiMenu.Open();
            MonoMain.pauseMenu = uiGroup;
        }
    }
}
