using CellMenu;
using HarmonyLib;
using NewUpdatedRundownProgression.ConfigFiles;
using NewUpdatedRundownProgression.ConfigFiles.Progression;
using NewUpdatedRundownProgression.PluginInfo;
using System.Text.Json;

namespace NewUpdatedRundownProgression.Patches
{
    [HarmonyPatch(typeof(CM_PageExpeditionSuccess), nameof(CM_PageExpeditionSuccess.OnEnable))]
    internal class PageSuccess_OnEnable
    {
        public static void Postfix(CM_PageExpeditionSuccess __instance) 
        {
            if (!__instance || GameStateManager.CurrentStateName == eGameStateName.Startup || GameStateManager.CurrentStateName == eGameStateName.NoLobby)
            {
                Logger.Error("Not a good state or page is null");
                return;
            }

            if (!RundownManager.TryGetIdFromLocalRundownKey(RundownManager.ActiveRundownKey, out uint rundownID))
            {
                Logger.Error("Could not get local ID from rundown key");
                return;
            }

            NewClearsFile mainClearFile = LoadClearData.GetClearData(rundownID);
            if (mainClearFile == null)
            {
                Logger.Error("Clear data was null");
                return;
            }

            string clearPath = LoadClearData.GetClearPath(rundownID);
            string publicName = RundownManager.ActiveExpedition.Descriptive.PublicName;

            ClearData levelClear;

            if (mainClearFile.ClearData.ContainsKey(publicName))
                levelClear = mainClearFile.GetClearDataForExpedition(publicName);
            else
            {
                levelClear = new ClearData();
                mainClearFile.ClearData.Add(publicName, levelClear);
            }

            pWardenObjectiveState currentState = WardenObjectiveManager.CurrentState;
            bool completedMain = currentState.main_status == eWardenObjectiveStatus.WardenObjectiveItemSolved;
            bool completedSecondary = currentState.second_status == eWardenObjectiveStatus.WardenObjectiveItemSolved;
            bool completedOverload = currentState.third_status == eWardenObjectiveStatus.WardenObjectiveItemSolved;
            bool PECompleted = completedMain && completedSecondary && completedOverload;

            Logger.Debug($"Attempting file serialization for completion: {completedMain} {completedSecondary} {completedOverload} {PECompleted}");

            if (!completedMain)
                return;

            if (levelClear.HighClears == 0)
                mainClearFile.AllClearsInRundown.HighClears++;

            levelClear.HighClears++;

            if (completedSecondary) 
            {
                if (levelClear.SecondaryClears == 0)
                    mainClearFile.AllClearsInRundown.SecondaryClears++;

                levelClear.SecondaryClears++;
            }
            if (completedOverload) 
            {
                if (levelClear.OverloadClears == 0)
                    mainClearFile.AllClearsInRundown.OverloadClears++;

                levelClear.OverloadClears++;
            }
            if (PECompleted) 
            {
                if (levelClear.PEClears == 0)
                    mainClearFile.AllClearsInRundown.PEClears++;

                levelClear.PEClears++;
            }

            string jsonOutput = JsonSerializer.Serialize(mainClearFile, EntryPoint.SerializerOptions);
            File.WriteAllText(clearPath, jsonOutput);
        }
    }
}
