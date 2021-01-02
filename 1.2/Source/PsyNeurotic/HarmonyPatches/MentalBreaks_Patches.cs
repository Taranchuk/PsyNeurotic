using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PsyNeurotic
{

    [HarmonyPatch(typeof(MentalBreakWorker), "CommonalityFor")]
    internal static class CommonalityFor_Patch
    {
        private static void Postfix(MentalBreakWorker __instance, ref float __result, Pawn pawn)
        {
            if (PsyUtils.TryGetPsyTrait(pawn, out PsyTraitDef traitDef))
            {
                if (__result > 0f && traitDef.allowedMentalBreaks.Count > 0 && !traitDef.allowedMentalBreaks.Contains(__instance.def))
                {
                    __result = 0f;
                }
            }
            else if (__instance.def == PsyDefOf.PN_Compulsion)
            {
                __result = 0f;
            }

            Log.Message(__instance.def + " - " + __result);
        }
    }
}
