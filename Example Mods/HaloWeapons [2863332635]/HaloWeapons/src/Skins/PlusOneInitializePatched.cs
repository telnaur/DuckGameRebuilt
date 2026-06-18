using HarmonyLib;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(PlusOne), nameof(PlusOne.Initialize))]
    internal static class PlusOneInitializePatched
    {
        [HarmonyPostfix]
        private static void Initialize(ref Profile ____profile)
        {
            if (____profile == Core.LocalProfile)
                Skins.AddCredits(50);
        }
    }
}
