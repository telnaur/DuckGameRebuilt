using HarmonyLib;
using System.Collections.Generic;

namespace DuckGame.HaloWeapons
{
    [HarmonyPatch(typeof(GhostObject), nameof(GhostObject.MaskIsMaxValue))]
	internal static class GhostObjectMaskIsMaxValuePatched
	{
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
		{
			return BindingAttributes.InsertReadMaskPatchInstructions(instructions);
		}
	}
}
