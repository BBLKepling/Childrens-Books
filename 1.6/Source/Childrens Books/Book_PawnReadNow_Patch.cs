using HarmonyLib;
using RimWorld;
using Verse;

namespace Childrens_Books
{
    [HarmonyPatch]
    public class Book_PawnReadNow_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn)
        {
            if (pawn?.CurJob?.def == JobDefOf.Reading && pawn.CurJob.targetA.Thing is Book book && book.def == ChildrensBookDefOf.BBLK_ColoringBook) pawn.CurJob.reportStringOverride = "BBLK_ChildrensBook_coloring".Translate(book.Named("BOOK"));
        }
    }
}
