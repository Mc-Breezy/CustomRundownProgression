using CellMenu;
using HarmonyLib;
using NewUpdatedRundownProgression.PluginInfo;

namespace NewUpdatedRundownProgression.Patches
{
    [HarmonyPatch(typeof(CM_PageRundown_New), nameof(CM_PageRundown_New.Setup))]
    internal class PageRundown_Setup
    {
        public static void Postfix() 
        {
            LoadClearData.Load();
            Logger.Debug("Loading Clear Data");
        }
    }
}
