using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Reflection.Emit.OpCodes;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(GhostObject), MethodType.Constructor, new Type[]
    {
        typeof(Thing),
        typeof(GhostManager),
        typeof(int),
        typeof(bool)
    })]
    internal static class GhostObjectConstructorPatched
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> code = instructions.ToList();

            for (int i = 0; i < code.Count; i++)
            {
                CodeInstruction instruction = code[i];

                if (instruction.opcode == Call && instruction.operand == AccessTools.Method(typeof(GhostObject), nameof(GhostObject.GetCurrentState)))
                {
                    code.InsertRange(i - 2, new List<CodeInstruction>
                    {
                        new CodeInstruction(Ldarg_0),
                        new CodeInstruction(Call, AccessTools.Method(typeof(BindingAttributes), nameof(BindingAttributes.InitializeStateBindings)))
                    });

                    break;
                }
            }

            return code;
        }
    }
}
