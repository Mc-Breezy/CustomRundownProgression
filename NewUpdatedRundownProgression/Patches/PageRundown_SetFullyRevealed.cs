using CellMenu;
using HarmonyLib;
using NewUpdatedRundownProgression.ConfigFiles;
using NewUpdatedRundownProgression.ConfigFiles.MainFile;
using NewUpdatedRundownProgression.PluginInfo;

namespace NewUpdatedRundownProgression.Patches
{
    [HarmonyPatch(typeof(CM_PageRundown_New), nameof(CM_PageRundown_New.SetRundownFullyRevealed))]
    internal class PageRundown_SetFullyRevealed
    {
        public static void Postfix(CM_PageRundown_New __instance) 
        {
            if (!__instance.m_rundownIsRevealed)
                return;
            ClearData allClears = LoadClearData.GetClearData(__instance.m_currentRundownData.persistentID).AllClearsInRundown;
            RundownManager.RundownProgData rundownProgression;

            if (allClears == null || !Utility.GetProgressionForRundown(__instance, out rundownProgression))
                return;

            if (!__instance.m_tierMarkerSectorSummary)
                return;

            Utility.SetSectorSummary(__instance, allClears, rundownProgression);

            CustomProgression customProgression = RundownProgressionFileSetup.GetCustomProgressionFile(__instance.m_currentRundownData.name);
            if (customProgression == null)
                return;

            __instance.m_tierMarker1.SetVisible(!customProgression.HideTierMarkers);
            __instance.m_tierMarker2.SetVisible(!customProgression.HideTierMarkers);
            __instance.m_tierMarker3.SetVisible(!customProgression.HideTierMarkers);
            __instance.m_tierMarker4.SetVisible(!customProgression.HideTierMarkers);
            __instance.m_tierMarker5.SetVisible(!customProgression.HideTierMarkers);

            Logger.Debug("The rundown is fully revealed");
        }
    }
}
