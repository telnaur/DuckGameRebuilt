using HarmonyLib;

namespace DuckGame.HaloWeapons
{
    internal static class DuckKillPatched
    {
        public static void Kill(Duck __instance)
        {
            if (Level.current is TeamSelect2)
                return;

            Profile localProfile = Core.LocalProfile;

            if (__instance.killedByProfile == localProfile && __instance.profile != localProfile)
                Skins.AddCredits(10);
        }
    }
}
