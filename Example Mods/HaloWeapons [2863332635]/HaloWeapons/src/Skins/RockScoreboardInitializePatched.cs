using HarmonyLib;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(RockScoreboard), nameof(RockScoreboard.Initialize))]
    internal static class RockScoreboardInitializePatched
    {
        [HarmonyPostfix]
        private static void Initialize(ref Team ____winningTeam)
        {
            if (____winningTeam == Core.LocalProfile.team)
                Skins.AddCredits(100);
        }
    }
}
