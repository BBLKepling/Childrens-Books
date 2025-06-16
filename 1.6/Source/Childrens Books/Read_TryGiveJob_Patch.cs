using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Childrens_Books
{
    [HarmonyPatch]
    public class Read_TryGiveJob_Patch
    {
        [HarmonyPostfix]
        public static Job Postfix(Job job)
        {
            if (job?.targetA.Thing is Book book && book.def == ChildrensBookDefOf.BBLK_ColoringBook) job.reportStringOverride = "BBLK_ChildrensBook_coloring".Translate(book.Named("BOOK"));
            return job;
        }
    }
}
