using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PsyNeurotic
{
	public class CompProperties_PsyNeurotic : CompProperties
	{
		public CompProperties_PsyNeurotic()
		{
			this.compClass = typeof(CompPsyNeurotic);
		}
	}


	public class CompPsyNeurotic : ThingComp
	{
		public CompProperties_PsyNeurotic Props => this.props as CompProperties_PsyNeurotic;
		public Pawn Pawn => this.parent as Pawn;
		public bool Active => PsyUtils.TryGetPsyTrait(Pawn, out PsyTraitDef trait) && trait != null;
		public PsyTraitDef PsyTrait => PsyUtils.TryGetPsyTrait(Pawn, out PsyTraitDef trait) ? trait : null;

		private int compulsionLevel;
		private int elevevationLevel;

		public static Dictionary<int, float> severityStages = new Dictionary<int, float>
		{
			{1, 0f},
			{2, 0.2f},
			{3, 0.4f},
			{4, 0.6f},
			{5, 0.8f},
			{6, 1f},
		};

		public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
            {
				var psylinkLevel = Pawn.GetPsylinkLevel();
				if (psylinkLevel > 0)
                {
					compulsionLevel = psylinkLevel;
					elevevationLevel = psylinkLevel;
                }
            }
        }
        public void TryGainNextCompulsionLevel()
        {
			if (compulsionLevel < 6 && !(compulsionLevel - elevevationLevel >= 1))
            {
				compulsionLevel++;
            }
        }
		public void TryGainNextElevationLevel()
		{
			if (elevevationLevel < 6 && compulsionLevel > elevevationLevel && Pawn.InspirationDef == PsyDefOf.PN_Inspired_Elevation)
			{
				elevevationLevel++;
				Pawn.mindState.inspirationHandler.EndInspiration(Pawn.Inspiration);
			}
		}

		public bool CanGainNextPsylink()
        {
			var psylinkLevel = Pawn.GetPsylinkLevel();
			if (psylinkLevel < 6 && psylinkLevel < compulsionLevel && psylinkLevel < elevevationLevel)
            {
				return true;
            }
			return false;
        }

		public void GainNextPsylink()
        {
			var offset = elevevationLevel - Pawn.GetPsylinkLevel();
			Log.Message("Gaining next psylink level: " + offset);
			Pawn.ChangePsylinkLevel(offset);
			var psylinkLevel = Pawn.GetPsylinkLevel();
			Log.Message(PsyTrait + " - " + PsyTrait.linkedHediff);
			if (PsyTrait?.linkedHediff != null && severityStages.TryGetValue(psylinkLevel, out float severity))
            {
				var hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(PsyTrait.linkedHediff);
				if (hediff == null)
                {
					hediff = HediffMaker.MakeHediff(PsyTrait.linkedHediff, Pawn);
					Pawn.health.AddHediff(hediff);
                }
				Log.Message(Pawn + " - " + Pawn.health.hediffSet.GetFirstHediffOfDef(PsyTrait.linkedHediff));
				hediff.Severity = severity;
			}
			else
            {
				foreach (var stage in severityStages)
                {
					Log.Message(psylinkLevel + " - " + stage.Key + " - " + stage.Value);
                }
            }
			elevevationLevel = 0;
			compulsionLevel = 0;
        }


		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref compulsionLevel, "compulsionLevel");
			Scribe_Values.Look(ref elevevationLevel, "elevevationLevel");
		}

        public override string CompInspectStringExtra()
        {
			if (this.Active)
            {
				var stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("PN.CurrentCompulsionLevel".Translate(this.compulsionLevel));
				stringBuilder.AppendLine("PN.CurrentElevationLevel".Translate(this.elevevationLevel));
				return stringBuilder.ToString().TrimEndNewlines();
			}
			else
            {
				return base.CompInspectStringExtra();
            }
		}
    }
}
