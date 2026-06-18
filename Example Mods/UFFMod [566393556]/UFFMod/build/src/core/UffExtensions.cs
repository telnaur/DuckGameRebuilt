using System;

namespace DuckGame.UFFMod
{
    internal static class UffExtensions
    {
        internal static bool IsDev(this Duck duck)
        {
            return UffMod.UffDevelopers != null
                && Steam.user != null
                && duck.profile != null
                && ((duck.profile.steamID == 0 && Array.Exists(UffMod.UffDevelopers, e => e == Steam.user.id))
                || (Array.Exists(UffMod.UffDevelopers, e => e == duck.profile.steamID) && duck.profile.steamID == Steam.user.id));
        }

        internal static byte PlayerIndex(this Duck duck)
        {
            return (byte)Persona.Number(duck.persona);
        }
    }
}
