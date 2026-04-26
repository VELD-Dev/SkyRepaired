using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SkyRepaired.Patches
{
    [HarmonyPatch(typeof(DistanceAttackItem))]
    public class DistanceAttackItem_Patches
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DistanceAttackItem), nameof(DistanceAttackItem.dealDamage))]
        public static IEnumerable<CodeInstruction> EnableWorldCollisionOnLasers(IEnumerable<CodeInstruction> instructions)
        {
            // IL sequence near the end of the call:
            //   ldarg.0
            //   ldfld ::dist     <- anchor
            //   ldc.i4.1
            //   ldc.i4.0         <- worldCollision = false  (target, index +3 from anchor start)
            //   callvirt dealDamage
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

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(DistanceAttackItem), nameof(DistanceAttackItem.dealDamage))]
        public static bool FixLaserWorldCollision(DistanceAttackItem __instance)
        {
            Damage.script.dealDamage(false,
                __instance.GetComponent<Collider>(),
                __instance.target,
                __instance.transform.position,
                2f,
                __instance.transform.forward, 
               Damage.Type.STANDARD,
               __instance.force,
               __instance.dist,
               true,
               true);
            return false;
        }
    }
}
