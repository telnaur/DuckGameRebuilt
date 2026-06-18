using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DuckGame.HaloWeapons
{
    public static class ModInteractions
    {
        private static readonly Dictionary<string, Action<Assembly>> s_interactions = new Dictionary<string, Action<Assembly>>();
        private static Map<ulong, string> s_modNames = new Map<ulong, string>();

        private static IEnumerable<Mod> ValidMods => ModLoader.accessibleMods.Where(mod => mod.configuration is not null);

        public static void LoadMods()
        {
            string path = Paths.Get("mods.json");

            if (!File.Exists(path))
                return;

            Dictionary<string, string> mods = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));

            foreach (KeyValuePair<string, string> mod in mods)
            {
                ulong id = ulong.Parse(mod.Key);

                if (s_modNames.ContainsKey(id))
                    continue;

                s_modNames.Add(id, mod.Value);
            }
        }

        public static bool Add(string modName, Action<Assembly> action)
        {
            if (s_interactions.ContainsKey(modName))
                return false;

            s_interactions.Add(modName, action);

            return true;
        }

        public static void OnModsLoaded()
        {
            foreach (Mod mod in ValidMods)
            {
                ModConfiguration configuration = mod.configuration;
                ulong id = configuration.assignedWorkshopID;

                if (!s_modNames.TryGetValue(id, out string name))
                    continue;

                if (s_interactions.TryGetValue(name, out Action<Assembly> action))
                {
                    try
                    {
                        action(configuration.assembly);
                    }
                    catch
                    {
                        Core.Log($"|RED|Failed to interact with mod {name}(id: {id})");
                    }
                }
            }
        }

        public static bool ModEnabled(string name)
        {
            if (!s_modNames.TryGetKey(name, out ulong id))
                return false;

            return ValidMods.Any(mod => mod.configuration.assignedWorkshopID == id);
        }
    }
}
