using RimWorld;
using Verse;

namespace Childrens_Books
{
    [DefOf]
    public class ChildrensBookDefOf
    {
        static ChildrensBookDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ChildrensBookDefOf));
        }

        [MayRequireBiotech]
        public static InteractionDef BBLK_BabyRead;

        [MayRequireBiotech]
        public static InteractionDef BBLK_BedTimeStory;

        [MayRequireBiotech]
        public static JobDef BBLK_Job_BedTimeStory;

        [MayRequireBiotech]
        public static JobDef BBLK_Job_BedTimeListen;

        [MayRequireBiotech]
        public static JobDef BBLK_Job_PlayRead;

        [MayRequireBiotech]
        public static TaleDef BBLK_BabyRead_Tale;

        [MayRequireBiotech]
        public static TaleDef BBLK_BedTimeStory_Tale;

        [MayRequireBiotech]
        public static ThingDef BBLK_ChildrensBook;

        //Vanilla
        public static ThingDef BBLK_ColoringBook;

        [MayRequireBiotech]
        public static ThoughtDef BBLK_BedTimeStory_Thought;

        [MayRequire("bblkepling.youngathearttrait")]
        public static TraitDef BBLK_BigKid;
    }
}
