using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static System.Reflection.Emit.OpCodes;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(SmallFire), nameof(SmallFire.Update))]
    internal static class SmallFireUpdatePatched
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();
            Type redFireType = typeof(RedFire);

            MethodInfo newFire = AccessTools.Method(typeof(SmallFire), nameof(SmallFire.New));
            MethodInfo add = AccessTools.Method(typeof(Level), nameof(Level.Add));

            for (int i = 1; i < code.Count; i++)
            {
                if (code[i].operand == add && code[i - 1].operand == newFire)
                {
                    code.InsertRange(i, new List<CodeInstruction>
                    {
                        new CodeInstruction(Dup),
                        new CodeInstruction(Ldarg_0),
                        new CodeInstruction(Call, AccessTools.Method(redFireType, nameof(RedFire.OnChildSpawned)))
                    });

                    break;
                }
            }

            return code;
        }
    }
}
