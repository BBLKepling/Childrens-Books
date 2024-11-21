using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Childrens_Books
{
    public class BabyPlayGiver_PlayRead : BabyPlayGiver
    {
        private static readonly List<Thing> TmpCandidates = new List<Thing>();

        public override bool CanDo(Pawn pawn, Pawn baby)
        {
            if (!baby.health.capacities.CapableOf(PawnCapacityDefOf.Hearing) ||
                !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight) ||
                !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking) ||
                (!pawn.IsCarryingPawn(baby) && !pawn.CanReserveAndReach(baby, PathEndMode.Touch, Danger.Some)) ||
                !TryGetChildrensBookToRead(pawn, out _)
                ) return false;
            return true;
        }

        public override Job TryGiveJob(Pawn pawn, Pawn baby)
        {
            if (!TryGetChildrensBookToRead(pawn, out Book book)) return null;
            Job job = JobMaker.MakeJob(def.jobDef, baby, book);
            job.count = 1;
            return job;
        }

        public static bool TryGetChildrensBookToRead(Pawn pawn, out Book book)
        {
            book = null;
            TmpCandidates.Clear();
            TmpCandidates.AddRange(from thing in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Book)
                                   where thing.def == ChildrensBookDefOf.BBLK_ChildrensBook && pawn.CanReserveAndReach(thing, PathEndMode.Touch, Danger.Some)
                                   select thing);
            TmpCandidates.AddRange(from thing in pawn.Map.listerThings.GetThingsOfType<Building_Bookcase>().SelectMany((Building_Bookcase x) => x.HeldBooks)
                                   where thing.def == ChildrensBookDefOf.BBLK_ChildrensBook && pawn.CanReserveAndReach(thing, PathEndMode.Touch, Danger.Some)
                                   select thing);
            if (TmpCandidates.NullOrEmpty()) return false;
            book = (Book)TmpCandidates.RandomElement();
            TmpCandidates.Clear();
            return true;
        }
    }
}
