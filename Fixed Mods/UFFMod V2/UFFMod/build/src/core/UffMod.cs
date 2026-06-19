using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Linq;

[assembly: AssemblyTitle("UFF Items Mod")]
[assembly: AssemblyCompany("AlexMdle, Flan, Garoslaw")]
[assembly: AssemblyDescription("An assortment of weapons, props, and some hats by a few guys from the UFF. http://www.fraxyhq.net/forums/index.php")]
[assembly: AssemblyVersion("1.1.0.0")]

namespace DuckGame.UFFMod
{
    public class UffMod : Mod
    {
        internal static UffMain Main { get; private set; }
        internal static string AssemblyName { get; private set; }

        private bool versionMismatch;
        private string lastVersionLocation;

        public static ulong[] UffDevelopers { get; private set; }

        /* uncomment this and the code in OnPreInitialize() to enable blacklist
        private ulong[] Blacklist { get; } = { 64-bit steam id #1,
            64-bit steam id #2,
            64-bit steam id #3,
            etc.,
        };
        */

        // The mod's priority; this property controls the load order of the mod.
        public override Priority priority
		{
			get { return base.priority; }
		}

		// This function is run before all mods are finished loading.
		protected override void OnPreInitialize()
		{
            /* uncomment for blacklist
            if (Array.Exists(Blacklist, e => e == Steam.user.id))
                System.Environment.Exit(0);
            */

            base.OnPreInitialize();
        }

        // This function is run after all mods are loaded.
        protected override void OnPostInitialize()
		{
            UffDevelopers = new ulong[]{
                76561198012351703, // flan
                76561197988194837, // alex
                76561198047164393, // garo
                76561198017552519, // pure
            };

            bool isDev = false;
            if (Steam.user != null)
                isDev = Array.Exists(UffDevelopers, e => e == Steam.user.id);

            // Undertale Hats
            Teams.core.teams.Add(new Team("humans", GetPath<UffMod>("hats\\human")));
            Teams.core.teams.Add(new Team("timecops", GetPath<UffMod>("hats\\sans")));
            Teams.core.teams.Add(new Team("best friends", GetPath<UffMod>("hats\\flowey")));
            Teams.core.teams.Add(new Team("dogs", GetPath<UffMod>("hats\\dogfoxes")));

            // UFF Hats
            Teams.core.teams.Add(new Team("slimes", GetPath<UffMod>("hats\\flan")));
            Teams.core.teams.Add(new Team("darkwings", GetPath<UffMod>("hats\\alex")));
            Teams.core.teams.Add(new Team("dinos", GetPath<UffMod>("hats\\spriter")));
            Teams.core.teams.Add(new Team("dreamers", GetPath<UffMod>("hats\\garo")));
            Teams.core.teams.Add(new Team("leopards", GetPath<UffMod>("hats\\garo2")));
            Teams.core.teams.Add(new Team("the duck knight", GetPath<UffMod>("hats\\shadowSpectre")));
            Teams.core.teams.Add(new Team("dr. dskulls", GetPath<UffMod>("hats\\drdskull")));
            Teams.core.teams.Add(new Team("bad habits", GetPath<UffMod>("hats\\haku")));
            Teams.core.teams.Add(new Team("imagine", GetPath<UffMod>("hats\\spider")));
            Teams.core.teams.Add(new Team("ayyliens", GetPath<UffMod>("hats\\ayy")));
            Teams.core.teams.Add(new Team("questions", GetPath<UffMod>("hats\\pure")));
            Teams.core.teams.Add(new Team("a hat", GetPath<UffMod>("hats\\aHat")));
            Teams.core.teams.Add(new Team("qpus", GetPath<UffMod>("hats\\parallelUniverses")));
            Teams.core.teams.Add(new Team("ahoge", GetPath<UffMod>("hats\\flahoge")));
            Teams.core.teams.Add(new Team("kero", GetPath<UffMod>("hats\\kero"), lockd: !isDev));

            base.OnPostInitialize();

            // sets the uffmod external directory
            string uffModDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UFFMod");
            if (!Directory.Exists(uffModDirectory))
                Directory.CreateDirectory(uffModDirectory);

            // checks whether to disable the update message
            lastVersionLocation = Path.Combine(uffModDirectory, "lastVersion.txt");
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            versionMismatch = (!File.Exists(lastVersionLocation) || !File.ReadAllLines(lastVersionLocation)[0].Equals(version));

            Thread thread = new Thread(ExecuteOnceLoaded);
            thread.Start();

            CreateUFFLevelPlaylist();
        }

        private byte[] GetMD5Hash(byte[] sourceBytes)
        {
            return new MD5CryptoServiceProvider().ComputeHash(sourceBytes);
        }

        private void CreateUFFLevelPlaylist()
        {
            string levelsLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DuckGame\\Levels\\UFF\\");
            if (!Directory.Exists(levelsLocation))
                Directory.CreateDirectory(levelsLocation);
            IList<string> levelList = new List<string>();

            // copy levels over
            foreach (string s in Directory.GetFiles(GetPath<UffMod>("levels")))
            {
                string saveString = AssemblyName + "\\content\\levels\\";
                string oldLocation = s.Replace('/', '\\');
                string newLocation = levelsLocation + oldLocation.Substring(oldLocation.IndexOf(saveString) + saveString.Length);
                if (!File.Exists(newLocation) || !GetMD5Hash(File.ReadAllBytes(oldLocation)).SequenceEqual(GetMD5Hash(File.ReadAllBytes(newLocation))))
                    File.Copy(oldLocation, newLocation, true);
                levelList.Add(newLocation);
            }

            // create playlist
            string uffPlaylistLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DuckGame\\Levels\\UFF Levels.play");
            XElement playlistElement = new XElement("playlist");
            foreach (string s in levelList)
            {
                XElement levelElement = new XElement("element", s.Replace('\\', '/'));
                playlistElement.Add(levelElement);
            }
            XDocument playlist = new XDocument();
            playlist.Add(playlistElement);
            string contents = playlist.ToString();
            if (string.IsNullOrWhiteSpace(contents))
                throw new Exception("Blank XML (" + uffPlaylistLocation + ")");
            if (!File.Exists(uffPlaylistLocation) || !File.ReadAllText(uffPlaylistLocation).Equals(contents))
            {
                File.WriteAllText(uffPlaylistLocation, contents);
                SaveCloudFile(uffPlaylistLocation, "Levels/UFF Levels.play");
            }
        }

        // pretty much just copied from DuckFile
        private void SaveCloudFile(string path, string localPath)
        {
            if (MonoMain.disableCloud || MonoMain.cloudNoSave || !Steam.IsInitialized())
                return;
            byte[] data = File.ReadAllBytes(path);
            Steam.FileWrite(localPath, data, data.Length);
        }

        private void ExecuteOnceLoaded()
        {
            while (Level.current == null
                    || !(Level.current.ToString() == "DuckGame.TitleScreen"
                    || Level.current.ToString() == "DuckGame.TeamSelect2"))
                Thread.Sleep(200);

            Main = new UffMain();

            if (versionMismatch)
            {
                OpenUpdateWindow();

                File.WriteAllLines(lastVersionLocation, new string[] { Assembly.GetExecutingAssembly().GetName().Version.ToString() });
            }

        }

        private void OpenUpdateWindow()
        {
            UIComponent uiGroup = new UIComponent(160f, 90f, 0, 0);
            UIMenu uiMenu = new UIMenu("@LWING@UFFMOD UPDATE@RWING@", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 230f);
            BitmapFont f = new BitmapFont("smallBiosFontUI", 7, 5);
            UIText[] uiTextArray = { new UIText("|DGRED|UFFMOD|WHITE| HAS BEEN UPDATED TO    ", Color.White),
                new UIText("VERSION 1.1. LOTS OF NEW ITEMS", Color.White),
                new UIText("AND MAPS HAVE BEEN ADDED!!   ", Color.White),
                new UIText("", Color.White),
                new UIText("MAPS WILL BE PACKAGED WITH THE", Color.White),
                new UIText("MOD FROM NOW ON-- NO NEED TO  ", Color.White),
                new UIText("SUBSCRIBE TO A COLLECTION NOW.", Color.White),
                new UIText("", Color.White),
                new UIText("IF YOU'VE SUBSCRIBED AFTER THE", Color.White),
                new UIText("UPDATE, YOU CAN IGNORE THIS.  ", Color.White),
                new UIText("", Color.White),
            };
            foreach (UIText uiText in uiTextArray)
            {
                uiText.SetFont(f);
                uiMenu.Add(uiText);
            }
            uiMenu.Add(new UIMenuItem("|DGGREEN|OK!", new UIMenuActionCloseMenu(uiGroup), backButton: true));
            uiGroup.Add(uiMenu, false);
            Level.Add(uiGroup);
            uiGroup.Open();
            uiMenu.Open();
            MonoMain.pauseMenu = uiGroup;
        }
    }
}
