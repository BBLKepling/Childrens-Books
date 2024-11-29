using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Childrens_Books
{
    public class JobDriver_PlayRead : JobDriver_BabyPlay
    {
        private bool isLearningDesire;
        protected Book Book => (Book)base.TargetThingB;
        protected LocalTargetInfo BookTarget => base.TargetB;

        protected override StartingConditions StartingCondition => StartingConditions.PickupBaby;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (base.TryMakePreToilReservations(errorOnFailed) && pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed)) return pawn.Reserve(job.GetTarget(TargetIndex.B), job, 1, 1, null, errorOnFailed);
            return false;
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            AddFailCondition(() => !Baby.health.capacities.CapableOf(PawnCapacityDefOf.Hearing));
            AddFailCondition(() => !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight));
            AddFailCondition(() => !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking));
            Toil failIfNoBookInInventory = FailIfNoBookInInventory();
            yield return Toils_Jump.JumpIf(failIfNoBookInInventory, () => !BookTarget.IsValid || pawn.inventory.Contains(Book));
            yield return Toils_Goto.GotoCell(Book.PositionHeld, PathEndMode.ClosestTouch).FailOnDestroyedOrNull(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, canTakeFromInventory: true);
            yield return Toils_General.PutCarriedThingInInventory();
            yield return failIfNoBookInInventory;
            foreach (Toil toil in base.MakeNewToils())
            {
                yield return toil;
            }
        }
        public override void Notify_Starting()
        {
            base.Notify_Starting();
            job.count = 1;
            isLearningDesire = pawn?.learning != null && pawn.learning.ActiveLearningDesires.Contains(LearningDesireDefOf.Reading);
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref isLearningDesire, "isLearningDesire", defaultValue: false);
        }

        protected override IEnumerable<Toil> Play()
        {
            yield return GoToChair();
            yield return SitInChair();
            yield return PlayToil();
        }
        protected Toil FailIfNoBookInInventory()
        {
            Toil toil = ToilMaker.MakeToil("FailIfNoBookInInventory");
            toil.FailOn(() => !pawn.inventory.Contains(Book));
            return toil;
        }
        protected Toil PlayToil()
        {
            Toil toil = ToilMaker.MakeToil("PlayToil");
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.handlingFacing = false;
            toil.WithEffect(EffecterDefOf.PlayStatic, TargetIndex.A);
            AddFinishAction(delegate
            {
                pawn.inventory.DropCount(Book.def, 1);
            });
            toil.tickAction = (Action)Delegate.Combine(toil.tickAction, (Action)delegate
            {
                if (Find.TickManager.TicksGame % 600 == 0)
                {
                    pawn.interactions.TryInteractWith(base.Baby, ChildrensBookDefOf.BBLK_BabyRead);
                }
                if (roomPlayGainFactor < 0f)
                {
                    roomPlayGainFactor = BabyPlayUtility.GetRoomPlayGainFactors(base.Baby);
                }
                if (isLearningDesire)
                {
                    LearningUtility.LearningTickCheckEnd(pawn);
                }
                if (BabyPlayUtility.PlayTickCheckEnd(base.Baby, pawn, roomPlayGainFactor + Book.JoyFactor - 1, Book))
                {
                    pawn.jobs.curDriver.ReadyForNextToil();
                }
            });
            ChildcareUtility.MakeBabyPlayAsLongAsToilIsActive(toil, TargetIndex.A);
            return toil;
        }
        protected Toil GoToChair()
        {
            Toil toil = ToilMaker.MakeToil("GoToChair");
            toil.initAction = delegate
            {
                IntVec3 readingSpot = RCellFinder.SpotToStandDuringJob(pawn);
                Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.OnCell, TraverseParms.For(pawn), 32f, (Thing t) => IsValidReadingChair(t) && (int)t.Position.GetDangerFor(pawn, t.Map) <= (int)readingSpot.GetDangerFor(pawn, pawn.Map));
                if (thing != null)
                {
                    Toils_Ingest.TryFindFreeSittingSpotOnThing(thing, pawn, out readingSpot);
                }
                pawn.ReserveSittableOrSpot(readingSpot, toil.actor.CurJob);
                pawn.Map.pawnDestinationReservationManager.Reserve(pawn, pawn.CurJob, readingSpot);
                pawn.pather.StartPath(readingSpot, PathEndMode.OnCell);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }

        protected Toil SitInChair()
        {
            Toil toil = ToilMaker.MakeToil("SitInChair");
            toil.initAction = delegate
            {
                Thing thing2;
                if (pawn.Spawned && (thing2 = pawn.Position.GetThingList(pawn.Map).FirstOrDefault((Thing thing) => IsValidReadingChair(thing))) != null)
                {
                    pawn.Rotation = thing2.Rotation;
                }
            };
            toil.defaultCompleteMode = ToilCompleteMode.Instant;
            return toil;
        }

        public bool IsValidReadingChair(Thing t)
        {
            if (t.def.building == null || 
                !t.def.building.isSittable || 
                !Toils_Ingest.TryFindFreeSittingSpotOnThing(t, pawn, out var _) || 
                t.IsForbidden(pawn) || 
                !pawn.CanReserve(t) || 
                !t.IsSociallyProper(pawn) || 
                t.IsBurning() ||
                t.HostileTo(pawn)
                ) return false;
            return true;
        }
    }
}
