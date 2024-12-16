using CellMenu;
using GameData;
using HarmonyLib;
using NewUpdatedRundownProgression.ConfigFiles;
using NewUpdatedRundownProgression.ConfigFiles.MainFile;
using NewUpdatedRundownProgression.ConfigFiles.Progression;
using NewUpdatedRundownProgression.PluginInfo;

namespace NewUpdatedRundownProgression.Patches
{
    [HarmonyPatch(typeof(CM_PageRundown_New), nameof(CM_PageRundown_New.UpdateExpeditionIconProgression))]
    internal class WorkingHack //:) (Don't know about any side effects from this LOL but oh well)
    {
        public static bool Prefix(CM_PageRundown_New __instance) 
        {
            if (!__instance) 
            {
                Logger.Error("Rundown page was Null?");
                return false;
            }

            if (!__instance.m_tierMarker1)
            {
                Logger.Error("Tier marker was null? Suck on this null ref!");
                return false;
            }

            __instance.UpdateTierIconsWithProgression(RundownManager.RundownProgression, default, __instance.m_expIconsTier1, __instance.m_tierMarker1, true);
            //__instance.UpdateTierIconsWithProgression(RundownManager.RundownProgression, default, __instance.m_expIconsTier2, __instance.m_tierMarker2, true);
            //__instance.UpdateTierIconsWithProgression(RundownManager.RundownProgression, default, __instance.m_expIconsTier3, __instance.m_tierMarker3, true);
            //__instance.UpdateTierIconsWithProgression(RundownManager.RundownProgression, default, __instance.m_expIconsTier4, __instance.m_tierMarker4, true);
            //__instance.UpdateTierIconsWithProgression(RundownManager.RundownProgression, default, __instance.m_expIconsTier5, __instance.m_tierMarker5, true);
            return false;
        }
    }
    //Runs once for each tier but CM_PageRundown_New.UpdateExpeditionIconProgression causes a null ref that does not seem to effect anything? Error only appears upon patching
    [HarmonyPatch(typeof(CM_PageRundown_New), nameof(CM_PageRundown_New.UpdateTierIconsWithProgression))] 
    internal class PageRundown_UpdateProgress
    {
        public static CM_PageRundown_New RundownPage => GuiManager.MainMenuLayer.PageRundownNew;

        private static int s_count = 0; //Prevents the mutliple runs from each tier call. No null ref call :)

        public static void Postfix(CM_PageRundown_New __instance) 
        {
            /*
            if (++s_count % 5 != 0)
                return;

            s_count = 0;
            */
            if (!__instance || GameStateManager.CurrentStateName == eGameStateName.Startup)
            {
                Logger.Error("Not a good state or is null");
                return;
            }

            RundownDataBlock rundownData = __instance.m_currentRundownData;
            NewClearsFile clearFile = LoadClearData.GetClearData(rundownData.persistentID);
            if (clearFile == null || clearFile.ClearData == null)
            {
                Logger.Error("All clears for rundown returned null");
                return;
            }

            Utility.SetSectorSummary(__instance, clearFile.AllClearsInRundown);

            CustomProgression customProgression = RundownProgressionFileSetup.GetCustomProgressionFile(rundownData.name);
            if (customProgression == null)
            {
                Logger.Error("Custom progression requirements are null");
                customProgression = new CustomProgression()
                {
                    TierARequirements = Utility.CreateDefaultTierRequirement(),
                    TierBRequirements = Utility.CreateDefaultTierRequirement(),
                    TierCRequirements = Utility.CreateDefaultTierRequirement(),
                    TierDRequirements = Utility.CreateDefaultTierRequirement(),
                    TierERequirements = Utility.CreateDefaultTierRequirement()
                };
            }

            RundownManager.RundownProgData rundownProgressionData = new RundownManager.RundownProgData()
            {
                clearedMain = clearFile.AllClearsInRundown.HighClears,
                clearedSecondary = clearFile.AllClearsInRundown.SecondaryClears,
                clearedThird = clearFile.AllClearsInRundown.OverloadClears,
                clearedAllClear = clearFile.AllClearsInRundown.PEClears
            };

            __instance.m_tierMarker1.SetProgression(rundownProgressionData, customProgression.TierARequirements);
            __instance.m_tierMarker2.SetProgression(rundownProgressionData, customProgression.TierBRequirements);
            __instance.m_tierMarker3.SetProgression(rundownProgressionData, customProgression.TierCRequirements);
            __instance.m_tierMarker4.SetProgression(rundownProgressionData, customProgression.TierDRequirements);
            __instance.m_tierMarker5.SetProgression(rundownProgressionData, customProgression.TierERequirements);

            foreach (CM_ExpeditionIcon_New icon in __instance.m_expIconsAll)
                EditIcon(icon, rundownData, customProgression, clearFile);

            Logger.Debug("Updating tier data");
        }

        private static void EditIcon(CM_ExpeditionIcon_New icon, RundownDataBlock block, CustomProgression progression, NewClearsFile clearFile) 
        {
            ExpeditionInTierData iconTierData = icon.DataBlock;
            icon.HideArtifactHeat();
            icon.m_statusText.transform.position = icon.m_artifactHeatText.transform.position;

            CustomTierRequirement customTierRequirement = progression.GetCustomEntry(icon.Tier, icon.ExpIndex);
            if (customTierRequirement == null)
            {
                Logger.Error($"Custom tier requirement was null for Tier: {icon.Tier} | Index: {icon.ExpIndex}");
                customTierRequirement = new CustomTierRequirement();
            }
            if (customTierRequirement.ChangePosition)
                icon.gameObject.transform.localPosition = customTierRequirement.NewPosition;

            if (customTierRequirement.LockData.HideExpedition)
            {
                icon.IsHidden = true;
                icon.SetVisible(false);
            }

            ClearData levelClearData;

            if (!clearFile.ClearData.ContainsKey(iconTierData.Descriptive.PublicName))
                levelClearData = new ClearData();
            else
                levelClearData = clearFile.GetClearDataForExpedition(iconTierData.Descriptive.PublicName);

            string[] clearText = new string[4]
            {
                levelClearData.HighClears.ToString(),
                RundownManager.HasSecondaryLayer(iconTierData) ? levelClearData.SecondaryClears.ToString() : "-",
                RundownManager.HasThirdLayer(iconTierData) ? levelClearData.OverloadClears.ToString() : "-",
                RundownManager.HasAllCompletetionPossibility(iconTierData) ? levelClearData.PEClears.ToString() : "-",
            };

            bool allRequiredExpeditionsCompleted = Utility.MeetsAllRequirements(customTierRequirement.LockData.Requirements, icon, clearFile);

            switch (customTierRequirement.LockData.LockType)
            {
                case ELockType.Default:
                    if (icon.Accessibility == eExpeditionAccessibility.BlockedAndScrambled && icon.Status == eExpeditionIconStatus.LockedAndScrambled)
                        return;

                    if (levelClearData.HighClears > 0)
                        SetIconStatus(icon, clearText, eExpeditionIconStatus.PlayedAndFinished, eExpeditionAccessibility.AlwaysAllow);
                    else
                        SetIconStatus(icon, clearText, eExpeditionIconStatus.PlayedNotFinished, eExpeditionAccessibility.AlwaysAllow);
                    break;
                case ELockType.UnlockedByTierClears:
                    if (clearFile.AllClearsInRundown.CheckTierRequirement(progression.GetTierProgressionData(icon.Tier)))
                    {
                        if (icon.Accessibility == eExpeditionAccessibility.BlockedAndScrambled && icon.Status == eExpeditionIconStatus.LockedAndScrambled)
                            return;
                        eExpeditionIconStatus status = levelClearData.HighClears > 0 ? eExpeditionIconStatus.PlayedAndFinished : eExpeditionIconStatus.PlayedNotFinished;
                        SetIconStatus(icon, clearText, status, eExpeditionAccessibility.AlwaysAllow);
                    }
                    else
                    {
                        SetIconStatus(icon, clearText, eExpeditionIconStatus.TierLocked, eExpeditionAccessibility.AlwayBlock);
                    }
                    break;
                case ELockType.UnlockedByOtherExpeditions:
                    if (allRequiredExpeditionsCompleted)
                    {
                        if (icon.Accessibility == eExpeditionAccessibility.BlockedAndScrambled && icon.Status == eExpeditionIconStatus.LockedAndScrambled)
                            return;
                        eExpeditionIconStatus status = levelClearData.HighClears > 0 ? eExpeditionIconStatus.PlayedAndFinished : eExpeditionIconStatus.PlayedNotFinished;
                        SetIconStatus(icon, clearText, status, eExpeditionAccessibility.AlwaysAllow);
                    }
                    else
                    {
                        SetIconStatus(icon, clearText, eExpeditionIconStatus.TierLocked, eExpeditionAccessibility.AlwayBlock);
                    }
                    break;
            }

            icon.LocalUpdate();
        }
        private static void SetIconStatus(CM_ExpeditionIcon_New icon, string[] clearText, eExpeditionIconStatus status, eExpeditionAccessibility accessibility)
        {
            icon.SetStatus(status, clearText[0], clearText[1], clearText[2], clearText[3]);
            icon.Accessibility = accessibility;
        }
    }
}
