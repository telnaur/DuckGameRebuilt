using HarmonyLib;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(MonoMain), nameof(MonoMain.KillEverything))]
    internal static class MonoMainKillEverythingPatched
    {
        [HarmonyPostfix]
        private static void KillEverything()
        {
            Skins.Save();
            Options.Save();
        }
    }
}
