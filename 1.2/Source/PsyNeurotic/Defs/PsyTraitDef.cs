using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PsyNeurotic
{
    public class PsyTraitDef : TraitDef
    {
        public List<SkillDef> conflictingSkills = new List<SkillDef>();
        public List<MentalBreakDef> allowedMentalBreaks = new List<MentalBreakDef>();
        public HediffDef linkedHediff;
    }
}
