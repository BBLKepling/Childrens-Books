using HarmonyLib;
using RimWorld;
using Verse;

namespace Childrens_Books
{
    [HarmonyPatch(typeof(LearningUtility), nameof(LearningUtility.LearningRateFactor))]
    public class LearningUtility_LearningRateFactor_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref float __result)
        {
            if (pawn?.CurJob?.targetA.Thing is Book book && (book.def == ChildrensBookDefOf.BBLK_ChildrensBook || book.def == ChildrensBookDefOf.BBLK_ColoringBook)) __result *= book.JoyFactor;
        }
    }
}
