using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PsyNeurotic
{
    [StaticConstructorOnStartup]
    public static class PsyUtils
    {
        static PsyUtils()
        {
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs.Where(x => x.race?.Humanlike ?? false))
            {
                thingDef.comps.Add(new CompProperties_PsyNeurotic());
            }
        }
        public static bool TryGetPsyTrait(Pawn pawn, out PsyTraitDef traitDef)
        {
            foreach (var trait in pawn.story?.traits?.allTraits)
            {
                if (trait.def is PsyTraitDef psyTrait)
                {
                    traitDef = psyTrait;
                    return true;
                }
            }
            traitDef = null;
            return false;
        }
    }
}
