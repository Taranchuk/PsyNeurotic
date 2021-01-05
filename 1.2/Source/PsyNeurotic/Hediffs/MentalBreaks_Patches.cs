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
    public class PsyHediff : Hediff
    {
        public override bool ShouldRemove => false;
    }
}
