using HarmonyLib;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(TitleScreen), nameof(TitleScreen.Initialize))]
    internal static class TitleScreenInitializePatched
    {
        [HarmonyPostfix]
        private static void Initialize()
        {
            Skins.ShowAddedCredits();
        }
    }
}
