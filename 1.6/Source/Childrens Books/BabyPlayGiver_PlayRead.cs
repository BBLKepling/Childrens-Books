using RimWorld;
using Verse;
using Verse.AI;

namespace Childrens_Books
{
    public class BabyPlayGiver_PlayRead : BabyPlayGiver
    {
        public override bool CanDo(Pawn pawn, Pawn baby)
        {
            return baby.health.capacities.CapableOf(PawnCapacityDefOf.Hearing) &&
                pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight) &&
                pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking) &&
                (pawn.IsCarryingPawn(baby) || pawn.CanReserveAndReach(baby, PathEndMode.Touch, Danger.Some)) &&
                ChildrensBookUtility.TryGetChildrensBookToRead(pawn, out _);
        }

        public override Job TryGiveJob(Pawn pawn, Pawn baby)
        {
            if (!ChildrensBookUtility.TryGetChildrensBookToRead(pawn, out Book book)) return null;
            Job job = JobMaker.MakeJob(def.jobDef, baby, book);
            job.count = 1;
            return job;
        }
    }
}
