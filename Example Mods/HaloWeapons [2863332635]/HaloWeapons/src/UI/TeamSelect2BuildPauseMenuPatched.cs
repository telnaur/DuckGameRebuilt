using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using static System.Reflection.Emit.OpCodes;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(TeamSelect2), nameof(TeamSelect2.BuildPauseMenu))]
    internal static class TeamSelect2BuildPauseMenuPatched
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();

            for (int i = 0; i < code.Count - 1; i++)
            {
                if (code[i].opcode == Ldc_I4_0 && code[i + 1].opcode == Stloc_1)
                {
                    code.InsertRange(i, new List<CodeInstruction>()
                    {
                        new CodeInstruction(Ldloc_0),
                        new CodeInstruction(Callvirt, AccessTools.PropertyGetter(typeof(UIDivider), nameof(UIDivider.leftSection))),

                        new CodeInstruction(Ldarg_0),
                        GetLoadFieldInstruction("_pauseMenu"),

                        new CodeInstruction(Ldarg_0),
                        GetLoadFieldInstruction("_pauseGroup"),

                        new CodeInstruction(Call, AccessTools.Method(typeof(UI), nameof(UI.AddSkinsItemToPauseMenu)))
                    });

                    break;
                }
            }

            return code;
        }

        private static CodeInstruction GetLoadFieldInstruction(string fieldName)
        {
            return new CodeInstruction(Ldfld, AccessTools.Field(typeof(TeamSelect2), fieldName));
        }
    }
}
