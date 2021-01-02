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
		public override void PostExposeData()
		{
			base.PostExposeData();
		}
	}
}
