﻿using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Childrens_Books
{
    public class JobDriver_BedTimeStory : JobDriver
    {
        private bool hasInInventory;

        private bool carrying;

        private bool isLearningDesire;

        private const TargetIndex ChildInd = TargetIndex.B;

        private const TargetIndex BookInd = TargetIndex.C;

        private Thing Bed => job.GetTarget(TargetIndex.A).Thing;

        private Pawn Child => (Pawn)job.GetTarget(TargetIndex.B).Thing;

        private Book Book => (Book)job.GetTarget(TargetIndex.C).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(ChildInd), job, 1, -1, null, errorOnFailed) && pawn.Reserve(job.GetTarget(BookInd), job, 1, 1, null, errorOnFailed);
        }
        public override void Notify_Starting()
        {
            base.Notify_Starting();
            job.count = 1;
            hasInInventory = pawn.inventory != null && pawn.inventory.Contains(Book);
            carrying = pawn?.carryTracker.CarriedThing == Book;
            isLearningDesire = Child?.learning != null && Child.learning.ActiveLearningDesires.Contains(LearningDesireDefOf.Reading);
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref hasInInventory, "hasInInventory", defaultValue: false);
            Scribe_Values.Look(ref carrying, "carrying", defaultValue: false);
            Scribe_Values.Look(ref isLearningDesire, "wasLearningDesire", defaultValue: false);
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            SetFinalizerJob(delegate (JobCondition condition)
            {
                if (!pawn.IsCarryingThing(Book))
                {
                    return null;
                }
                if (condition != JobCondition.Succeeded)
                {
                    pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Direct, out var _);
                    return null;
                }
                return HaulAIUtility.HaulToStorageJob(pawn, Book);
            });
            AddFailCondition(() => !Child.health.capacities.CapableOf(PawnCapacityDefOf.Hearing));
            AddFailCondition(() => !Child.InBed());
            AddFailCondition(() => !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation));
            AddFailCondition(() => !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight));
            AddFailCondition(() => !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking));
            Toil failIfNoBook = FailIfNoBook();
            yield return Toils_Jump.JumpIf(failIfNoBook, () => !Book.Position.IsValid || pawn.IsCarryingThing(Book));
            if (!carrying)
            {
                if (hasInInventory)
                {
                    yield return Toils_Misc.TakeItemFromInventoryToCarrier(pawn, TargetIndex.C);
                }
                else
                {
                    yield return Toils_Goto.GotoCell(Book.PositionHeld, PathEndMode.ClosestTouch).FailOnDestroyedOrNull(TargetIndex.C).FailOnSomeonePhysicallyInteracting(TargetIndex.C);
                    yield return Toils_Haul.StartCarryThing(TargetIndex.C, canTakeFromInventory: true);
                }
            }
            yield return failIfNoBook;
            yield return Toils_Goto.GotoCell(Child.PositionHeld, PathEndMode.ClosestTouch).FailOnDestroyedOrNull(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return GoToChair();
            yield return ReadToil(job.def.joyDuration);
        }
        protected Toil FailIfNoBook()
        {
            Toil toil = ToilMaker.MakeToil("FailIfNoBook");
            toil.FailOn(() => !pawn.IsCarryingThing(Book));
            return toil;
        }
        protected Toil ReadToil(int duration)
        {
            Toil toil = Toils_General.Wait(duration);
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.handlingFacing = true;
            toil.initAction = delegate
            {
                Book.IsOpen = true;
                pawn.pather.StopDead();
                job.showCarryingInspectLine = false;
                pawn.rotationTracker.FaceCell(job.GetTarget(TargetIndex.B).Cell);
                Child.jobs.StopAll();
                Job kidjob = JobMaker.MakeJob(ChildrensBookDefOf.BBLK_Job_BedTimeListen, Bed, pawn);
                Child.jobs.StartJob(kidjob);
            };
            toil.tickAction = delegate
            {
                if (Find.TickManager.TicksGame % 600 == 0)
                {
                    pawn.interactions.TryInteractWith(Child, ChildrensBookDefOf.BBLK_BedTimeStory);
                }
                pawn.GainComfortFromCellIfPossible();
                if (pawn.CurJob != null && pawn.needs?.joy != null)
                {
                    JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.None, Book.JoyFactor * BookUtility.GetReadingBonus(pawn));
                }
                if ((isLearningDesire && LearningUtility.LearningTickCheckEnd(Child)) || ticksLeftThisToil <= 0) pawn.jobs.curDriver.ReadyForNextToil();
                if (pawn.IsHashIntervalTick(600))
                {
                    pawn.jobs.CheckForJobOverride(9.1f);
                }
            };
            toil.AddEndCondition(() => BookUtility.CanReadBook(Book, pawn, out var _) ? JobCondition.Ongoing : JobCondition.InterruptForced);
            toil.AddFinishAction(delegate
            {
                Book.IsOpen = false;
                job.showCarryingInspectLine = true;
                Child.jobs.StopAll();
                Job kidjob = JobMaker.MakeJob(JobDefOf.LayDown, Bed);
                Child.jobs.StartJob(kidjob);
                if (ticksLeftThisToil > 1000) return;
                Book.TryGetQuality(out QualityCategory qc);
                Child.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(ChildrensBookDefOf.BBLK_BedTimeStory_Thought, (int)qc));
                //Needs DefOf
                //TaleRecorder.RecordTale(ChildrensBookDefOf.BBLK_BedTimeStory_Tale, pawn, Child);
            });
            return toil;
        }
        protected Toil GoToChair()
        {
            Toil toil = ToilMaker.MakeToil("GoToChair");
            toil.initAction = delegate
            {
                IntVec3 readingSpot = pawn.Position;
                Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.OnCell, TraverseParms.For(pawn), 3f, (Thing t) => IsValidReadingChair(t) && (int)t.Position.GetDangerFor(pawn, t.Map) <= (int)readingSpot.GetDangerFor(pawn, pawn.Map));
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