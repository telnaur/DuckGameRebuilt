using HarmonyLib;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(TeamSelect2), nameof(TeamSelect2.Initialize))]
    internal static class TeamSelect2InitializePatched
    {
        [HarmonyPostfix]
        private static void Initialize()
        {
            Skins.ShowAddedCredits();
        }
    }
}
