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
    [StaticConstructorOnStartup]
    internal static class HarmonyInit
    {
        static HarmonyInit()
        {
            new Harmony("PsyNeurotic.HarmonyInit").PatchAll();
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
