using RimWorld;
using System;

namespace Childrens_Books
{
    public class BookOutcomeProperties_ChildrensBook : BookOutcomeProperties
    {
        public override Type DoerClass => typeof(ReadingOutcomeDoerChildrensBook);
    }
}
