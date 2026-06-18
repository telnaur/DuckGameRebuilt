using HarmonyLib;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(SpriteMap), nameof(SpriteMap.DrawWithoutUpdate))]
    internal static class SpriteMapDrawWithoutUpdatePatched
    {
        [HarmonyPrefix]
        private static bool OnDrawStarted(SpriteMap __instance)
        {
            SpriteMaterials.OnSpriteDrawStart(__instance);

            return true;
        }

        [HarmonyPostfix]
        private static void OnDrawFinished()
        {
            SpriteMaterials.OnSpriteDrawFinished();
        }
    }
}
