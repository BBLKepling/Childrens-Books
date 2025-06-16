using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Childrens_Books
{
    public class JobDriver_BedTimeListen : JobDriver_LayDownAwake
    {
        public override bool LookForOtherJobs => false;
        private Pawn Reader => (Pawn)job.GetTarget(TargetIndex.B).Thing;
        protected override IEnumerable<Toil> MakeNewToils()
        {
            AddFailCondition(() => Reader.CurJob.def != ChildrensBookDefOf.BBLK_Job_BedTimeStory);
            foreach (Toil toil in base.MakeNewToils())
            {
                yield return toil;
            }
        }
        public override string GetReport()
        {
            return "BBLK_ChildrensBook_Listen".Translate(Reader.Named("PAWN"));
        }
    }
}
