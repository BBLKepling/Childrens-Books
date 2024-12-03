using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Childrens_Books
{
    [HarmonyPatch]
    public class LearningGiver_Reading_TryGiveJob_Patch
    {
        private static readonly List<Thing> Babies = new List<Thing>();
        [HarmonyPrefix]
        public static bool Prefix(ref Job __result, Pawn pawn)
        {
            if (Rand.Bool || 
                !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight) ||
                !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking)) return true;
            Babies.Clear();
            Babies.AddRange(from thing in pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Pawn)
                                   where IsValidBaby(thing)
                                   select thing);
            if (Babies.NullOrEmpty()) return true;
            Pawn baby = (Pawn)Babies.RandomElement();
            Babies.Clear();
            if (!ChildrensBookUtility.TryGetChildrensBookToRead(pawn, out Book book)) return true;
            __result = JobMaker.MakeJob(ChildrensBookDefOf.BBLK_Job_PlayRead, baby, book);
            __result.count = 1;
            return false;
            bool IsValidBaby(Thing t)
            {
                return t is Pawn p && 
                    p.Awake() && 
                    p.health.capacities.CapableOf(PawnCapacityDefOf.Hearing) && 
                    p.needs?.play?.CurLevel is float f && 
                    f <= 0.9f &&
                    (pawn.IsCarryingPawn(p) || pawn.CanReserveAndReach(p, PathEndMode.Touch, Danger.Some));
            }
        }
    }
}
