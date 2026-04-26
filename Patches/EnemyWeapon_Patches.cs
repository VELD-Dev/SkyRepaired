using System.Reflection.Emit;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SkyRepaired.Patches
{

    [HarmonyPatch(typeof(EnemyWeapon))]
    public class EnemyWeapon_Patches
    {
        // Not sure if this is the right way to fix it, but we'll see for now.
        // I patched this because the laser is way too fast, partially because it's raycasted, but also because the delay
        // between when the enemy shoots and when the damage is applied is way too short.
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(EnemyWeapon), nameof(EnemyWeapon.checkDistanceAttack))]
        public static IEnumerable<CodeInstruction> RaiseLaserDamageDelay(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);
            CodeMatcher matcher = new CodeMatcher(instructions);

            // Matcher, hopefully it's how it works
            matcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldloc_0),
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), "GetComponent", generics: new Type[] { typeof(DistanceAttackItem) } )),
                new CodeMatch(OpCodes.Ldloc_1),
                new CodeMatch(OpCodes.Ldc_R4, 0.1f)
                );

            // Check if the pattern was found before trying to modify it
            if (matcher.IsValid)
            {
                // Change the laser damage delay from 0.1f to 0.35f
                matcher.Advance(3)
                    .RemoveInstruction()
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_R4, Plugin.Config.LaserDamageDelay.Value));
                Plugin.Logger.LogInfo("[RaiseLaserDamageDelay] Successfully replaced laser damage delay.");
            }
            else
            {
                Plugin.Logger.LogError("[RaiseLaserDamageDelay] Couldn't match the IL !");
            }

            matcher.Start();

            return matcher.Instructions();
        }
    }
}