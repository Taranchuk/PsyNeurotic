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
	public class JobGiver_Compulsion : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			var comp = pawn.TryGetComp<CompPsyNeurotic>();
			if (comp is null || !comp.Active)
            {
				return null;
            }
			var trait = comp.PsyTrait;
			if (trait == PsyDefOf.PN_PsyNeuroticWarrior || trait == PsyDefOf.PN_PsyNeuroticMarksman)
            {
				return TryGetJob(pawn, new JobGiver_AIFightEnemies());
			}
			else if (trait == PsyDefOf.PN_PsyNeuroticQuarryman)
            {
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Mining");
				return GetJobFor(pawn, workType.workGiversByPriority);
			}
			else if (trait == PsyDefOf.PN_PsyNeuroticArchitect)
			{
				var workType = WorkTypeDefOf.Construction;
				return GetJobFor(pawn, workType.workGiversByPriority);
			}
			else if (trait == PsyDefOf.PN_PsyNeuroticGourmet)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Cooking");
				return GetJobFor(pawn, workType.workGiversByPriority);
			}
			else if (trait == PsyDefOf.PN_PsyNeuroticFarmer)
            {
				var growingWorkType = DefDatabase<WorkTypeDef>.GetNamed("Growing");
				var plantCuttingWorkType = DefDatabase<WorkTypeDef>.GetNamed("PlantCutting");
				var workgivers = new List<WorkGiverDef>();
				workgivers.AddRange(growingWorkType.workGiversByPriority);
				workgivers.AddRange(plantCuttingWorkType.workGiversByPriority);
				return GetJobFor(pawn, workgivers);
			}
			else if (trait == PsyDefOf.PN_PsyNeuroticRancher)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Handling");
				return GetJobFor(pawn, workType.workGiversByPriority);
			}
			else if (trait == PsyDefOf.PN_PsyNeuroticTinkerer)
            {
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Crafting");
				return GetJobFor(pawn, workType.workGiversByPriority);
			}
			else if (trait == PsyDefOf.PN_PsyNeuroticDaVinci)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Crafting");
				return GetJobFor(pawn, workType.workGiversByPriority);
			}
			else if (trait == PsyDefOf.PN_PsyNeuroticDoctor)
			{
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Doctor");
				return GetJobFor(pawn, workType.workGiversByPriority);
			}
			else if (trait == PsyDefOf.PN_PsyNeuroticCeasar)
            {
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Warden");
				return GetJobFor(pawn, workType.workGiversByPriority);
			}
			else if (trait == PsyDefOf.PN_PsyNeuroticEinstein)
            {
				var workType = DefDatabase<WorkTypeDef>.GetNamed("Research");
				return GetJobFor(pawn, workType.workGiversByPriority);
			}
			return null;
		}

		private Job TryGetJob(Pawn pawn, ThinkNode_JobGiver jobGiver)
        {
			jobGiver.ResolveReferences();
			var result = jobGiver.TryIssueJobPackage(pawn, default(JobIssueParams));
			if (result.Job != null)
			{
				return result.Job;
			}
			return null;
		}

		private static List<WorkGiverDef> AllowedWorkGiversFor(Pawn pawn, List<WorkGiverDef> workGiverDefs)
		{
			List<WorkGiverDef> allowedWorkGiverDefs = new List<WorkGiverDef>();
			foreach (var workGiver in workGiverDefs)
			{
				if (!pawn.WorkTypeIsDisabled(workGiver.workType))
				{
					allowedWorkGiverDefs.Add(workGiver);
				}
			}
			return allowedWorkGiverDefs;
		}
		public static Job GetJobFor(Pawn pawn, List<WorkGiverDef> workGiverDefs)
		{
			List<WorkGiver> list = AllowedWorkGiversFor(pawn, workGiverDefs).Select(x => x.Worker).ToList();
			int num = -999;
			TargetInfo bestTargetOfLastPriority = TargetInfo.Invalid;
			WorkGiver_Scanner scannerWhoProvidedTarget = null;
			WorkGiver_Scanner scanner;
			IntVec3 pawnPosition;
			float closestDistSquared;
			float bestPriority;
			bool prioritized;
			bool allowUnreachable;
			Danger maxPathDanger;
			for (int j = 0; j < list.Count; j++)
			{
				WorkGiver workGiver = list[j];
				if (workGiver.def.priorityInType != num && bestTargetOfLastPriority.IsValid)
				{
					break;
				}
				if (!PawnCanUseWorkGiver(pawn, workGiver))
				{
					continue;
				}
				try
				{
					Job job2 = workGiver.NonScanJob(pawn);
					if (job2 != null)
					{
						return job2;
					}
					scanner = (workGiver as WorkGiver_Scanner);
					if (scanner != null)
					{
						if (scanner.def.scanThings)
						{
							Predicate<Thing> validator = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t);
							IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
							Thing thing;
							if (scanner.Prioritized)
							{
								IEnumerable<Thing> enumerable2 = enumerable;
								if (enumerable2 == null)
								{
									enumerable2 = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
								}
								thing = ((!scanner.AllowUnreachable) ? GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, enumerable2, scanner.PathEndMode, TraverseParms.For(pawn, scanner.MaxPathDanger(pawn)), 9999f, validator, (Thing x) => scanner.GetPriority(pawn, x)) : GenClosest.ClosestThing_Global(pawn.Position, enumerable2, 99999f, validator, (Thing x) => scanner.GetPriority(pawn, x)));
							}
							else if (scanner.AllowUnreachable)
							{
								IEnumerable<Thing> enumerable3 = enumerable;
								if (enumerable3 == null)
								{
									enumerable3 = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
								}
								thing = GenClosest.ClosestThing_Global(pawn.Position, enumerable3, 99999f, validator);
							}
							else
							{
								thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, scanner.PotentialWorkThingRequest, scanner.PathEndMode, TraverseParms.For(pawn, scanner.MaxPathDanger(pawn)), 9999f, validator, enumerable, 0, scanner.MaxRegionsToScanBeforeGlobalSearch, enumerable != null);
							}
							if (thing != null)
							{
								bestTargetOfLastPriority = thing;
								scannerWhoProvidedTarget = scanner;
							}
						}
						if (scanner.def.scanCells)
						{
							pawnPosition = pawn.Position;
							closestDistSquared = 99999f;
							bestPriority = float.MinValue;
							prioritized = scanner.Prioritized;
							allowUnreachable = scanner.AllowUnreachable;
							maxPathDanger = scanner.MaxPathDanger(pawn);
							IEnumerable<IntVec3> enumerable4 = scanner.PotentialWorkCellsGlobal(pawn);
							IList<IntVec3> list2;
							if ((list2 = (enumerable4 as IList<IntVec3>)) != null)
							{
								for (int k = 0; k < list2.Count; k++)
								{
									ProcessCell(list2[k]);
								}
							}
							else
							{
								foreach (IntVec3 item in enumerable4)
								{
									ProcessCell(item);
								}
							}
						}
					}
					void ProcessCell(IntVec3 c)
					{
						bool flag = false;
						float num2 = (c - pawnPosition).LengthHorizontalSquared;
						float num3 = 0f;
						if (prioritized)
						{
							if (!c.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, c))
							{
								if (!allowUnreachable && !pawn.CanReach(c, scanner.PathEndMode, maxPathDanger))
								{
									return;
								}
								num3 = scanner.GetPriority(pawn, c);
								if (num3 > bestPriority || (num3 == bestPriority && num2 < closestDistSquared))
								{
									flag = true;
								}
							}
						}
						else if (num2 < closestDistSquared && !c.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, c))
						{
							if (!allowUnreachable && !pawn.CanReach(c, scanner.PathEndMode, maxPathDanger))
							{
								return;
							}
							flag = true;
						}
						if (flag)
						{
							bestTargetOfLastPriority = new TargetInfo(c, pawn.Map);
							scannerWhoProvidedTarget = scanner;
							closestDistSquared = num2;
							bestPriority = num3;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(pawn, " threw exception in WorkGiver ", workGiver.def.defName, ": ", ex.ToString()));
				}
				finally
				{
				}
				if (bestTargetOfLastPriority.IsValid)
				{
					Job job3 = (!bestTargetOfLastPriority.HasThing) ? scannerWhoProvidedTarget.JobOnCell(pawn, bestTargetOfLastPriority.Cell) : scannerWhoProvidedTarget.JobOnThing(pawn, bestTargetOfLastPriority.Thing);
					if (job3 != null)
					{
						job3.workGiverDef = scannerWhoProvidedTarget.def;
						return job3;
					}
				}
				num = workGiver.def.priorityInType;
			}
			return null;
		}

		private static bool PawnCanUseWorkGiver(Pawn pawn, WorkGiver giver)
		{
			if (!giver.def.nonColonistsCanDo && !pawn.IsColonist)
			{
				return false;
			}
			if (pawn.WorkTagIsDisabled(giver.def.workTags))
			{
				return false;
			}
			if (giver.ShouldSkip(pawn))
			{
				return false;
			}
			if (giver.MissingRequiredCapacity(pawn) != null)
			{
				return false;
			}
			return true;
		}
	}
}
