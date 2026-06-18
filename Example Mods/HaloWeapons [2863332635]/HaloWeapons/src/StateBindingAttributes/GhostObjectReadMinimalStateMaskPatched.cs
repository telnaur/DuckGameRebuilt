using HarmonyLib;
using System;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(GhostObject), nameof(GhostObject.ReadMinimalStateMask))]
    internal static class GhostObjectReadMinimalStateMaskPatched
    {
        [HarmonyPrefix]
        private static bool ReadMinimalStateMask(Type t, BitBuffer b, ref long __result)
        {
            int count = Editor.AllStateFields[t].Length;
            BindingAttributes.CorrectBindingsCount(t, ref count);

            __result = b.ReadBits<long>(count);

            return false;
        }
    }
}
