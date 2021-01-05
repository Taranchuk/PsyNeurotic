using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PsyNeurotic
{

    [HarmonyPatch(typeof(Pawn), "Kill")]
    internal static class Kill_Patch
    {
        private static void Postfix(DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            if (dinfo.HasValue && dinfo.Value.Instigator is Pawn pawn && pawn.InspirationDef == PsyDefOf.PN_Inspired_Elevation)
            {
                var comp = pawn.TryGetComp<CompPsyNeurotic>();
                if (comp != null && comp.Active)
                {
                    comp.TryGainNextElevationLevel();
                    if (comp.CanGainNextPsylink())
                    {
                        comp.GainNextPsylink();
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(SkillRecord), "CalculateTotallyDisabled")]
    internal static class CalculateTotallyDisabled_Patch
    {
        private static void Postfix(SkillRecord __instance, Pawn ___pawn, ref bool __result)
        {
            if (!__result && PsyUtils.TryGetPsyTrait(___pawn, out PsyTraitDef traitDef) && traitDef.conflictingSkills.Contains(__instance.def))
            {
                __result = true;
            }
        }
    }
}
