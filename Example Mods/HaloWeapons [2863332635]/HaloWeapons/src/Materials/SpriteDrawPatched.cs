using HarmonyLib;
using System;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(Sprite), nameof(Sprite.Draw), new Type[]
    {

    })]
    internal static class SpriteDrawPatched
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
