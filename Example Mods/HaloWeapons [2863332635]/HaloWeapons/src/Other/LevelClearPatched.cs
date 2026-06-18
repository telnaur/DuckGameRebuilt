using HarmonyLib;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(Level), nameof(Level.Clear))]
    internal static class LevelClearPatched
    {
        [HarmonyPostfix]
        private static void Clear()
        {
            SpriteMaterials.Clear();
            RedFire.Clear();
        }
    }
}
