using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Childrens_Books
{
    public class JobDriver_PlayRead : JobDriver_BabyPlay
    {
        private bool hasInInventory;

        private bool carrying;

        private bool isLearningDesire;
        protected Book Book => (Book)base.TargetThingB;
        protected LocalTargetInfo BookTarget => base.TargetB;

        protected override StartingConditions StartingCondition => StartingConditions.PickupBaby;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed) && pawn.Reserve(job.GetTarget(TargetIndex.B), job, 1, 1, null, errorOnFailed);
        }
        public override void Notify_Starting()
        {
            base.Notify_Starting();
            job.count = 1;
            hasInInventory = pawn.inventory != null && pawn.inventory.Contains(Book);
            carrying = pawn?.carryTracker.CarriedThing == Book;
            isLearningDesire = pawn?.learning != null && pawn.learning.ActiveLearningDesires.Contains(LearningDesireDefOf.Reading);
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref hasInInventory, "hasInInventory", defaultValue: false);
            Scribe_Values.Look(ref carrying, "carrying", defaultValue: false);
            Scribe_Values.Look(ref isLearningDesire, "isLearningDesire", defaultValue: false);
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            SetFinalizerJob(delegate (JobCondition condition)
            {
                if (!pawn.inventory.Contains(Book))
                {
                    return null;
                }
                pawn.inventory.DropCount(Book.def, 1);
                return null;
            });
            AddFailCondition(() => !Baby.health.capacities.CapableOf(PawnCapacityDefOf.Hearing));
            AddFailCondition(() => !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation));
            AddFailCondition(() => !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight));
            AddFailCondition(() => !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking));
            Toil failIfNoBookInInventory = FailIfNoBookInInventory();
            yield return Toils_Jump.JumpIf(failIfNoBookInInventory, () => !BookTarget.IsValid || pawn.inventory.Contains(Book));
            if (!carrying && !hasInInventory)
            {
                yield return Toils_Goto.GotoCell(Book.PositionHeld, PathEndMode.ClosestTouch).FailOnDestroyedOrNull(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
                yield return Toils_Haul.StartCarryThing(TargetIndex.B, canTakeFromInventory: true);
            }
            if (!hasInInventory) yield return Toils_General.PutCarriedThingInInventory();
            yield return failIfNoBookInInventory;
            foreach (Toil toil in base.MakeNewToils())
            {
                yield return toil;
            }
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
                if (pawn.inventory.Contains(Book)) pawn.inventory.DropCount(Book.def, 1);
                TaleRecorder.RecordTale(ChildrensBookDefOf.BBLK_BabyRead_Tale, pawn, Baby, Book);
            });
            toil.initAction = delegate
            {
                Baby.CurJob.reportStringOverride = "BBLK_ChildrensBook_Listen".Translate(pawn.Named("PAWN"));
            };
            toil.tickIntervalAction = delegate (int delta)
            {
                if (pawn.RaceProps.Humanlike && ticksLeftThisToil % 600 == 0)
                {
                    pawn.interactions.TryInteractWith(base.Baby, ChildrensBookDefOf.BBLK_BabyRead);
                }
                pawn.GainComfortFromCellIfPossible(delta);
                if (roomPlayGainFactor < 0f)
                {
                    roomPlayGainFactor = BabyPlayUtility.GetRoomPlayGainFactors(base.Baby) + Book.JoyFactor - 1;
                }
                if (isLearningDesire)
                {
                    LearningUtility.LearningTickCheckEnd(pawn, delta);
                }
                if (BabyPlayUtility.PlayTickCheckEnd(base.Baby, pawn, roomPlayGainFactor, delta, Book))
                {
                    pawn.jobs.curDriver.ReadyForNextToil();
                }
            };
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
