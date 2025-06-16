using RimWorld;
using System;

namespace Childrens_Books
{
    public class BookOutcomeProperties_ColoringBook : BookOutcomeProperties
    {
        public override Type DoerClass => typeof(ReadingOutcomeDoerColoringBook);
    }
}
