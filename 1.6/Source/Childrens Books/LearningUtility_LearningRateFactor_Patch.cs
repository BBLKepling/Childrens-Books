using HarmonyLib;
using RimWorld;
using Verse;

namespace Childrens_Books
{
    [HarmonyPatch]
    public class LearningUtility_LearningRateFactor_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref float __result)
        {
            if (pawn?.CurJob?.targetA.Thing is Book bookA && (bookA.def == ChildrensBookDefOf.BBLK_ChildrensBook || bookA.def == ChildrensBookDefOf.BBLK_ColoringBook)) __result += bookA.JoyFactor - 1;
            else if (pawn?.CurJob?.targetB.Thing is Book bookB && (bookB.def == ChildrensBookDefOf.BBLK_ChildrensBook || bookB.def == ChildrensBookDefOf.BBLK_ColoringBook)) __result += bookB.JoyFactor - 1;
        }
    }
}
