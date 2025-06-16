using UnityEngine;
using Verse;

namespace Childrens_Books
{
    public class ChildrensBookMod : Mod
    {
        public ChildrensBookMod(ModContentPack content) : base(content)
        {
            GetSettings<ChildrensBookSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            ChildrensBookSettings.colorbookFactor = (int)listingStandard.SliderLabeled("BBLK_ChildrensBook_colorbookFactor_Label".Translate() + " " + ChildrensBookSettings.colorbookFactor + "%", ChildrensBookSettings.colorbookFactor, 50, 300, tooltip: "BBLK_ChildrensBook_colorbookFactor_ToolTip".Translate());
            listingStandard.CheckboxLabeled("BBLK_ChildrensBook_boredMessege_Label".Translate(), ref ChildrensBookSettings.boredMessege, "BBLK_ChildrensBook_boredMessege_ToolTip".Translate());
            ChildrensBookSettings.boredTicks = (int)listingStandard.SliderLabeled("BBLK_ChildrensBook_boredTicks_Label".Translate() + " " + ChildrensBookSettings.boredTicks, ChildrensBookSettings.boredTicks, 1, 60, tooltip: "BBLK_ChildrensBook_boredTicks_ToolTip".Translate());
            listingStandard.CheckboxLabeled("BBLK_ChildrensBook_colorbookMessege_Label".Translate(), ref ChildrensBookSettings.colorbookMessege, "BBLK_ChildrensBook_colorbookMessege_ToolTip".Translate());
            ChildrensBookSettings.colorbookTick = (int)listingStandard.SliderLabeled("BBLK_ChildrensBook_colorbookTick_Label".Translate() + " " + ChildrensBookSettings.colorbookTick, ChildrensBookSettings.colorbookTick, 1, 60, tooltip: "BBLK_ChildrensBook_colorbookTick_ToolTip".Translate());
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory() => "BBLK_ChildrensBook_Settings".Translate();
    }
}
