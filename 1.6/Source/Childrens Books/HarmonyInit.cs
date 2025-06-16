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

            MethodInfo original1 = AccessTools.Method(typeof(JoyGiver_Read), nameof(JoyGiver_Read.TryGiveJob));
            MethodInfo postfix1 = AccessTools.Method(typeof(Read_TryGiveJob_Patch), nameof(Read_TryGiveJob_Patch.Postfix));
            harmonyInstance.Patch(original: original1, postfix: new HarmonyMethod(postfix1));

            MethodInfo original2 = AccessTools.Method(typeof(Book), nameof(Book.PawnReadNow));
            MethodInfo postfix2 = AccessTools.Method(typeof(Book_PawnReadNow_Patch), nameof(Book_PawnReadNow_Patch.Postfix));
            harmonyInstance.Patch(original: original2, postfix: new HarmonyMethod(postfix2));

            if (ModsConfig.BiotechActive)
            {
                MethodInfo original3 = AccessTools.Method(typeof(LearningUtility), nameof(LearningUtility.LearningRateFactor));
                MethodInfo postfix3 = AccessTools.Method(typeof(LearningUtility_LearningRateFactor_Patch), nameof(LearningUtility_LearningRateFactor_Patch.Postfix));
                harmonyInstance.Patch(original: original3, postfix: new HarmonyMethod(postfix3));

                MethodInfo original4 = AccessTools.Method(typeof(LearningGiver_Reading), nameof(LearningGiver_Reading.TryGiveJob));
                MethodInfo prefix = AccessTools.Method(typeof(LearningGiver_Reading_TryGiveJob_Patch), nameof(LearningGiver_Reading_TryGiveJob_Patch.Prefix));
                //reuse postfix1
                harmonyInstance.Patch(original: original4, prefix: new HarmonyMethod(prefix), postfix: new HarmonyMethod(postfix1));
            }
        }
    }
}
