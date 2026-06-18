using HarmonyLib;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(Duck), nameof(Duck.ResetNonServerDeathState))]
    internal static class DuckResetNonServerDeathStatePatched
    {
        private const float DefaultMaxRunSpeed = 3.1f;

        [HarmonyPostfix]
        private static void ResetNonServerDeathState(Duck __instance)
        {
            __instance.runMax = DefaultMaxRunSpeed;
        }
    }
}
