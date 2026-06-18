using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using static System.Reflection.Emit.OpCodes;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(DuckNetwork), "OpenMenu")]
    internal static class DuckNetworkOpenMenuPatched
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();

            for (int i = 0; i < code.Count; i++)
            {
                object operand = code[i].operand;

                if (operand is not string line)
                    continue;

                if (line == "men5")
                {
                    code.Insert(i + 1, new CodeInstruction(Call, AccessTools.Method(typeof(UI), nameof(UI.AdSkinsItemToDucknetMenu))));

                    break;
                }
            }

            return code;
        }
    }
}
