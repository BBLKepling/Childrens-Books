using Verse;

namespace Childrens_Books
{
    public class ChildrensBookSettings : ModSettings
    {
        public static int colorbookFactor = 125;
        public static bool boredMessege = true;
        public static int boredTicks = 5;
        public static bool colorbookMessege = true;
        public static int colorbookTick = 5;
        public override void ExposeData()
        {
            Scribe_Values.Look(ref colorbookFactor, "colorbookFactor", 125);
            Scribe_Values.Look(ref boredMessege, "boredMessege", true);
            Scribe_Values.Look(ref boredTicks, "boredTicks", 5);
            Scribe_Values.Look(ref colorbookMessege, "colorbookMessege", true);
            Scribe_Values.Look(ref colorbookTick, "colorbookTick", 5);
            base.ExposeData();
        }
    }
}
