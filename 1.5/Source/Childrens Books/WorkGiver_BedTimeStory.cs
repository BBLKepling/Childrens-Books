using RimWorld;
using Verse.AI;
using Verse;
using System.Collections.Generic;

namespace Childrens_Books
{
    public class WorkGiver_BedTimeStory : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn teacher)
        {
            return teacher.Map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer);
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !ModsConfig.BiotechActive || !SchoolUtility.CanTeachNow(pawn);
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return pawn.learning == null &&
                t is Pawn pawn2 && 
                pawn.CanReserveAndReach(pawn2, PathEndMode.Touch, Danger.Some) &&
                pawn2.learning != null &&
                pawn2.InBed() &&
                pawn2.needs?.rest?.CurLevel is float f &&
                f < 0.35f &&
                pawn2.needs.mood?.thoughts?.memories is MemoryThoughtHandler mth &&
                mth.GetFirstMemoryOfDef(ChildrensBookDefOf.BBLK_BedTimeStory_Thought) == null &&
                pawn2.health?.capacities is PawnCapacitiesHandler pch &&
                pch.CapableOf(PawnCapacityDefOf.Hearing) &&
                ChildrensBookUtility.TryGetChildrensBookToRead(pawn, out _);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!(t is Pawn pawn2) || !ChildrensBookUtility.TryGetChildrensBookToRead(pawn, out Book book)) return null;
            return JobMaker.MakeJob(ChildrensBookDefOf.BBLK_Job_BedTimeStory, pawn2.CurJob.GetTarget(TargetIndex.A), pawn2, book);
        }
    }
}
