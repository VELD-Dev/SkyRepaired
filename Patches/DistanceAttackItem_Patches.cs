using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace SkyRepaired.Patches
{
    [HarmonyPatch(typeof(DistanceAttackItem))]
    public class DistanceAttackItem_Patches
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DistanceAttackItem), nameof(DistanceAttackItem.dealDamage))]
        public static IEnumerable<CodeInstruction> EnableWorldCollisionOnLasers(IEnumerable<CodeInstruction> instructions)
        {
            if(!Plugin.Config.LaserWorldCollision.Value)
            {
                Plugin.Logger.LogInfo("[EnableWorldCollisionOnLasers] Laser world collision is disabled in config, skipping patch.");
                return instructions;
            }

            FieldInfo distField = AccessTools.Field(typeof(DistanceAttackItem), "dist");
            CodeMatcher matcher = new CodeMatcher(instructions);
            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, distField),
                new CodeMatch(OpCodes.Ldc_I4_1),
                new CodeMatch(OpCodes.Ldc_I4_0)
            );

            if (matcher.IsValid)
            {
                matcher.Advance(3)
                       .RemoveInstruction()
                       .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_1));
                Plugin.Logger.LogInfo("[EnableWorldCollisionOnLasers] Successfully patched worldCollision to true.");
            }
            else
            {
                Plugin.Logger.LogError("[EnableWorldCollisionOnLasers] Pattern not found — worldCollision could not be patched.");
            }

            return matcher.Instructions();
        }
    }
}
