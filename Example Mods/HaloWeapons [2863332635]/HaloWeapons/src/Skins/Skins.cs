using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DuckGame.HaloWeapons
{
    public static class Skins
    {
        private static readonly Dictionary<Type, Skin[]> s_allSkins = new Dictionary<Type, Skin[]>();
        private static readonly Dictionary<Type, Vec2> s_spriteSizes = new Dictionary<Type, Vec2>();

        private static readonly JsonSerializer s_serializer = JsonSerializer.CreateDefault();

        private static SkinsData s_data;
        private static int s_addedCredits;

        public static int Credits => s_data.Credits;
        public static int CreditsToAdd => s_addedCredits;  

        private static string DataFileName => DuckFile.optionsDirectory + "skins.json";

        public static void SubtractCredits(int value)
        {
            if (value < 0)
                return;

            s_data.Credits = Math.Max(s_data.Credits - value, 0);
        }

        public static void AddCredits(int value)
        {
            if (value < 0)
                return;

            s_addedCredits += value;
            s_data.Credits += value;

            Core.Log($"|DGBLUE|{Core.LocalProfile.name} got |YELLOW|{value} |DGBLUE|credits!");
        }

        public static void ShowAddedCredits()
        {
            if (s_addedCredits < 1)
                return;

            UI.AddCreditIncreaseDisplay(s_data.Credits - s_addedCredits);

            s_addedCredits = 0;
        }

        public static bool BuySkin(Type type, Skin skin)
        {
            int cost = skin.Cost;

            if (cost > Credits)
                return false;

            int index = skin.Index;

            if (HasSkin(type, index))
                return false;

            Dictionary<Type, int[]> ownedSkins = s_data.OwnedSkins;

            if (!ownedSkins.ContainsKey(type))
            {
                ownedSkins.Add(type, new int[]
                {
                    index
                });
            }
            else
            {
                ownedSkins[type] = ownedSkins[type].Append(index).ToArray();
            }

            s_data.Credits -= cost;

            return true;
        }

        public static bool TryGetEquippedIndex(Type type, out int index)
        {
            if (s_data.EquippedSkins.TryGetValue(type, out index))
                return true;

            return false;
        }

        public static bool HasSkin(Type type, int index)
        {
            if (s_data.OwnedSkins.TryGetValue(type, out int[] indices))
                return indices.Contains(index);

            return false;
        }

        public static bool Equip(Type type, int index)
        {
            if (!HasSkin(type, index))
                return false;

            Dictionary<Type, int> equippedSkins = s_data.EquippedSkins;

            if (equippedSkins.ContainsKey(type))
                equippedSkins[type] = index;
            else
                equippedSkins.Add(type, index);

            return true;
        }

        public static bool Unequip(Type type)
        {
            Dictionary<Type, int> equippedSkins = s_data.EquippedSkins;

            if (equippedSkins.ContainsKey(type))
            {
                equippedSkins.Remove(type);
                return true;
            }

            return false;
        }

        public static bool IsEquipped(Type type, int index)
        {
            if (TryGetEquippedIndex(type, out int equippedIndex))
                return equippedIndex == index;

            return false;
        }

        public static string GetSkinPath(Type type, int index)
        {
            if (s_allSkins.TryGetValue(type, out Skin[] skins))
                foreach (Skin skin in skins)
                    if (skin.Index == index)
                        return GetSkinPath(type, skin.FileName);   

            return null;
        }

        public static string GetSkinPath(Type type, string fileName)
        {
            return Paths.GetSpritePath($"{type.Name}/{fileName}");
        }

        public static Sprite CreateDemoSprite(Type type, string spritePath)
        {
            if (s_spriteSizes.TryGetValue(type, out Vec2 size))
                return new SpriteMap(spritePath, (int)size.x, (int)size.y);

            return new Sprite(spritePath);
        }

        public static string GetDefaultSkinPath(Type type)
        {
            return Paths.GetSpritePath($"{type.Name}/default.png");
        }

        public static void Save()
        {
            using (FileStream stream = File.OpenWrite(DataFileName))
            {
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter)
                    {
                        Formatting = Formatting.Indented,
                    })
                    {
                        s_serializer.Serialize(jsonWriter, s_data);
                    }
                }
            }
        }

        public static void Load()
        {
            LoadSaveData();
            LoadWeaponsInfo();
        }

        public static IEnumerable<Type> GetSkinWeaponTypes()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(HaloWeapon))))
                if (!type.IsAbstract && type.GetCustomAttribute<NoSkinsAttribute>() is null)
                    yield return type;
        }

        public static IEnumerable<Skin> GetAll(Type type)
        {
            if (s_allSkins.TryGetValue(type, out Skin[] skins))
                return skins;

            return Enumerable.Empty<Skin>();
        }

        private static void LoadSaveData()
        {
            if (!File.Exists(DataFileName))
            {
                LoadDefaultSkinsData();
                return;
            }

            using (StreamReader streamReader = new StreamReader(DataFileName))
            using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                s_data = s_serializer.Deserialize<SkinsData>(jsonReader);

            if (s_data is null)
                LoadDefaultSkinsData();
        }

        private static void LoadWeaponsInfo()
        {
            string path = Paths.Get("skins.json");

            if (!File.Exists(path))
                return;

            JObject jObject = JObject.Parse(File.ReadAllText(path));

            foreach (Type type in GetSkinWeaponTypes())
            {
                JToken weapon = jObject[type.Name.ToLower()];

                if (weapon is null)
                    continue;

                s_allSkins.Add(type, ParseSkins(weapon["skins"]).ToArray());
                s_spriteSizes.Add(type, new Vec2((int)weapon["width"], (int)weapon["height"]));
            }
        }

        private static IEnumerable<Skin> ParseSkins(JToken skins)
        {
            for (int i = 0; i < skins.Count(); i++)
            {
                JToken skin = skins[i];

                string fileName = $"{skin["filename"]}.png";
                string name = (string)skin["name"];
                int cost = (int)skin["cost"];

                yield return new Skin(i, fileName, cost, name);
            }
        }

        private static void LoadDefaultSkinsData()
        {
            s_data = new SkinsData();
        }
    }
}
                