using HarmonyLib;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(CorptronLogo), nameof(CorptronLogo.Initialize))]
    internal static class CorptronLogoInitializePatched
    {
        [HarmonyPostfix]
        private static void Initialize()
        {
            ModInteractions.OnModsLoaded();
        }
    }
}
