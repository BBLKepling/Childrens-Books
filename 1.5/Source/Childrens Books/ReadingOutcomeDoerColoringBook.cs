using RimWorld;
using Verse;

namespace Childrens_Books
{
    public class ReadingOutcomeDoerColoringBook : BookOutcomeDoer
    {
        public new BookOutcomeProperties_ColoringBook Props => (BookOutcomeProperties_ColoringBook)props;
        public override bool DoesProvidesOutcome(Pawn reader)
        {
            return true;
        }
        public override void OnBookGenerated(Pawn author = null)
        {
            base.Book.SetJoyFactor(BookUtility.GetNovelJoyFactorForQuality(base.Quality) * (float)ChildrensBookSettings.colorbookFactor / 100);
        }

        public override void OnReadingTick(Pawn reader, float factor)
        {
            if (Find.TickManager.TicksGame % (ChildrensBookSettings.colorbookTick * 60) != 0) return;
            if (Parent.HitPoints > 1)
            {
                --Parent.HitPoints;
                return;
            }
            FilthMaker.TryMakeFilth(reader.Position, reader.Map, ThingDefOf.Filth_Trash);
            if (ChildrensBookSettings.colorbookMessege) Messages.Message("BBLK_ChildrensBook_ColoringBook".Translate(reader.Named("PAWN"), Parent.Named("BOOK")), Parent, MessageTypeDefOf.NeutralEvent);
            Parent.Destroy();
        }
        public override string GetBenefitsString(Pawn reader = null)
        {
            return string.Format(" - {0}: x{1}", ModsConfig.BiotechActive ? "BBLK_ChildrensBook_LearnJoyFactor".Translate() : "BookJoyFactor".Translate(), base.Book.JoyFactor.ToStringPercent());
        }
    }
}
