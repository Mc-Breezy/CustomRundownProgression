using NewUpdatedRundownProgression.ConfigFiles.MainFile;
using NewUpdatedRundownProgression.ConfigFiles.Progression;

namespace NewUpdatedRundownProgression.PluginInfo
{
    internal class OnLevelEnter
    {
        public static void EnterLevel() 
        {
            RundownManager.TryGetIdFromLocalRundownKey(RundownManager.ActiveRundownKey, out uint rundownID);
            NewClearsFile clearData = LoadClearData.GetClearData(rundownID);

            if (clearData == null)
            {
                Logger.Error("Clear data was null");
                return;
            }

            CustomProgression customProgression = RundownProgressionFileSetup.GetCustomProgressionFile(rundownID);

            if (customProgression == null)
            {
                Logger.Error("The custom progression was null");
                return;
            }

            pActiveExpedition activeExpedition = RundownManager.GetActiveExpeditionData();
            CustomTierRequirement req = customProgression.GetCustomEntry(activeExpedition.tier, activeExpedition.expeditionIndex);

            if (req == null) 
            {
                Logger.Error("The custom progression requirement was null");
                return;
            }

            Utility.EvaluateRequirements(req, clearData);
        }
    }
}
