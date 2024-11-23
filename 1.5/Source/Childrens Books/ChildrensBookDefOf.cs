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
        public static ThingDef BBLK_ChildrensBook;
        public static ThingDef BBLK_ColoringBook;
        [MayRequire("bblkepling.youngathearttrait")]
        public static TraitDef BBLK_BigKid;
    }
}
