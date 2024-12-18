﻿using RimWorld;
using Verse;

namespace Childrens_Books
{
    public class ReadingOutcomeDoerChildrensBook : BookOutcomeDoer
    {
        public new BookOutcomeProperties_ChildrensBook Props => (BookOutcomeProperties_ChildrensBook)props;

        public override bool DoesProvidesOutcome(Pawn reader)
        {
            return true;
        }
        public override void OnBookGenerated(Pawn author = null)
        {
            base.Book.SetJoyFactor(BookUtility.GetNovelJoyFactorForQuality(base.Quality));
        }
        public override void OnReadingTick(Pawn reader, float factor)
        {
            if (reader.learning != null ||
                Find.TickManager.TicksGame % (ChildrensBookSettings.boredTicks * 60) != 0 ||
                (ModLister.HasActiveModWithName("Young at Heart Trait") && reader.story.traits.HasTrait(ChildrensBookDefOf.BBLK_BigKid))
                ) return;
            if (ChildrensBookSettings.boredMessege) Messages.Message("BBLK_ChildrensBook_Bored".Translate(reader.Named("PAWN"), Parent.Named("BOOK")), Parent, MessageTypeDefOf.NeutralEvent);
            reader.jobs.StopAll();
        }
        public override string GetBenefitsString(Pawn reader = null)
        {
            return string.Format(" - {0}: x{1}", ModsConfig.BiotechActive ? "BBLK_ChildrensBook_LearnJoyFactor".Translate() : "BookJoyFactor".Translate(), base.Book.JoyFactor.ToStringPercent());
        }
    }
}
