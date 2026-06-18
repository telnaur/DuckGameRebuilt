using HarmonyLib;
using System;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(SpriteMap), nameof(SpriteMap.Draw), new Type[]
    {

    })]
    internal static class SpriteMapDrawPatched
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
