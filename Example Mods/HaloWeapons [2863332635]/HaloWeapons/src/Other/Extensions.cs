using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DuckGame.HaloWeapons
{
    internal static class Extensions
    {
        public static T GetValue<T>(this FieldInfo fieldInfo, object instance)
        {
            if (fieldInfo is null)
                return default(T);

            return (T)fieldInfo.GetValue(instance);
        }

        public static IEnumerable<CodeInstruction> InsertBetween(this IEnumerable<CodeInstruction> codeInstructions, OpCode first, OpCode second, IEnumerable<CodeInstruction> insert)
        {
            List<CodeInstruction> instructions = codeInstructions.ToList();
            
            for (int i = 1; i < instructions.Count; i++)
            {
                if (instructions[i].opcode == second && instructions[i - 1].opcode == first)
                {
                    instructions.InsertRange(i, insert);
                    break;
                }
            }

            return instructions;
        }

        public static IEnumerable<CodeInstruction> InsertBetween(this IEnumerable<CodeInstruction> codeInstructions, object first, object second, IEnumerable<CodeInstruction> insert)
        {
            List<CodeInstruction> instructions = codeInstructions.ToList();

            for (int i = 1; i < instructions.Count; i++)
            {
                if (instructions[i].operand == second && instructions[i - 1].operand == first)
                {
                    instructions.InsertRange(i, insert);
                    break;
                }
            }

            return instructions;
        }

        public static Vec2 GetTruePosition(this Duck duck)
        {
            Ragdoll ragdoll = duck.ragdoll;

            if (ragdoll is not null)
                return ragdoll.part2.position;

            return duck.position;
        }
    }
}
