﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimEffect
{
	public class Building_GalaxyMap : Building_CommsConsole
	{
		private CompPowerTrader powerComp;
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			powerComp = GetComp<CompPowerTrader>();
		}
		private FloatMenuOption GetFailureReason(Pawn myPawn)
		{
			if (!myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Some))
			{
				return new FloatMenuOption("CannotUseNoPath".Translate(), null);
			}
			if (base.Spawned && base.Map.gameConditionManager.ElectricityDisabled(base.Map))
			{
				return new FloatMenuOption("CannotUseSolarFlare".Translate(), null);
			}
			if (powerComp != null && !powerComp.PowerOn)
			{
				return new FloatMenuOption("CannotUseNoPower".Translate(), null);
			}
			if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking))
			{
				return new FloatMenuOption("CannotUseReason".Translate("IncapableOfCapacity".Translate(PawnCapacityDefOf.Talking.label, myPawn.Named("PAWN"))), null);
			}
			if (!CanUseCommsNow)
			{
				Log.Error(string.Concat(myPawn, " could not use comm console for unknown reason."));
				return new FloatMenuOption("Cannot use now", null);
			}
			return null;
		}

		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
		{
			FloatMenuOption failureReason = GetFailureReason(myPawn);
			if (failureReason != null)
			{
				yield return failureReason;
			}
			else
			{
				foreach (ICommunicable commTarget in GetCommTargets(myPawn))
				{
					FloatMenuOption floatMenuOption = commTarget.CommFloatMenuOption(this, myPawn);
					if (floatMenuOption != null)
					{
						yield return floatMenuOption;
					}
				}
			}

			foreach (var comp in this.AllComps)
            {
				foreach (var floatOption in comp.CompFloatMenuOptions(myPawn))
                {
					yield return floatOption;
                }
            }
		}

		public new void GiveUseCommsJob(Pawn negotiator, ICommunicable target)
		{
			Job job = JobMaker.MakeJob(RE_DefOf.RE_UseCommsConsole, this);
			job.commTarget = target;
			negotiator.jobs.TryTakeOrderedJob(job);
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.OpeningComms, KnowledgeAmount.Total);
		}
	}
}
