using HarmonyLib;
using Verse;

namespace Childrens_Books
{
    [StaticConstructorOnStartup]
    public class HarmonyInit
    {
        static HarmonyInit()
        {
            if (ModsConfig.BiotechActive)
            {
                Harmony harmonyInstance = new Harmony("BBLKepling.ChildrensBooks");
                harmonyInstance.PatchAll();
            }
        }
    }
}
