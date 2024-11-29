using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace Childrens_Books
{
    [StaticConstructorOnStartup]
    public class HarmonyInit
    {
        static HarmonyInit()
        {
            Harmony harmonyInstance = new Harmony("BBLKepling.ChildrensBooks");

            MethodInfo postfix1 = AccessTools.Method(typeof(Read_TryGiveJob_Patch), nameof(Read_TryGiveJob_Patch.Postfix));
            MethodInfo original1 = AccessTools.Method(typeof(JoyGiver_Read), nameof(JoyGiver_Read.TryGiveJob));
            harmonyInstance.Patch(original: original1, postfix: new HarmonyMethod(postfix1));

            MethodInfo postfix2 = AccessTools.Method(typeof(Book_PawnReadNow_Patch), nameof(Book_PawnReadNow_Patch.Postfix));
            MethodInfo original2 = AccessTools.Method(typeof(Book), nameof(Book.PawnReadNow));
            harmonyInstance.Patch(original: original2, postfix: new HarmonyMethod(postfix2));

            if (ModsConfig.BiotechActive)
            {
                MethodInfo postfix3 = AccessTools.Method(typeof(LearningUtility_LearningRateFactor_Patch), nameof(LearningUtility_LearningRateFactor_Patch.Postfix));
                MethodInfo original3 = AccessTools.Method(typeof(LearningUtility), nameof(LearningUtility.LearningRateFactor));
                harmonyInstance.Patch(original: original3, postfix: new HarmonyMethod(postfix3));

                //reuse postfix1
                MethodInfo original4 = AccessTools.Method(typeof(LearningGiver_Reading), nameof(LearningGiver_Reading.TryGiveJob));
                harmonyInstance.Patch(original: original4, postfix: new HarmonyMethod(postfix1));
            }
        }
    }
}
