using GameData;
using NewUpdatedRundownProgression.ConfigFiles.MainFile;
using NewUpdatedRundownProgression.ConfigFiles.Progression;
using NewUpdatedRundownProgression.ConfigFiles;
using CellMenu;
using NewUpdatedRundownProgression.Patches;

namespace NewUpdatedRundownProgression
{
    internal class Utility
    {
        public static RundownTierProgressionData CreateDefaultTierRequirement() => new()
        {
            MainSectors = 0,
            SecondarySectors = 0,
            ThirdSectors = 0,
            AllClearedSectors = 0
        };

        public static bool GetProgressionForRundown(out RundownManager.RundownProgData rundownProgression)
        {
            rundownProgression = new RundownManager.RundownProgData();

            if (!PageRundown_UpdateProgress.RundownPage)
                return false;

            for (int i = 0; i < PageRundown_UpdateProgress.RundownPage.m_expIconsAll.Count; i++)
            {
                CM_ExpeditionIcon_New icon = PageRundown_UpdateProgress.RundownPage.m_expIconsAll[i];

                if (!icon.gameObject.active)
                    continue;

                rundownProgression.totalMain++;
                if (RundownManager.HasSecondaryLayer(icon.DataBlock))
                    rundownProgression.totalSecondary++;
                if (RundownManager.HasThirdLayer(icon.DataBlock))
                    rundownProgression.totalThird++;
                if (RundownManager.HasAllCompletetionPossibility(icon.DataBlock))
                    rundownProgression.totalAllClear++;
            }

            return true;
        }

        public static bool GetProgressionForRundown(CM_PageRundown_New pageRundown, out RundownManager.RundownProgData rundownProgression)
        {
            rundownProgression = new RundownManager.RundownProgData();

            if (!pageRundown)
                return false;

            for (int i = 0; i < pageRundown.m_expIconsAll.Count; i++)
            {
                CM_ExpeditionIcon_New icon = pageRundown.m_expIconsAll[i];

                if (icon.IsHidden)
                    continue;

                rundownProgression.totalMain++;
                if (RundownManager.HasSecondaryLayer(icon.DataBlock))
                    rundownProgression.totalSecondary++;
                if (RundownManager.HasThirdLayer(icon.DataBlock))
                    rundownProgression.totalThird++;
                if (RundownManager.HasAllCompletetionPossibility(icon.DataBlock))
                    rundownProgression.totalAllClear++;
            }

            return true;
        }

        public static void EvaluateRequirements(CustomTierRequirement expInfo, NewClearsFile clearFile)
        {
            if (expInfo.WardenEventsOnLand == null)
                return;

            for (int i = 0; i < expInfo.WardenEventsOnLand.Count; i++)
            {
                ProgressionWardenEvent progressionEvent = expInfo.WardenEventsOnLand[i];
                if (CheckRequirementsMet(progressionEvent.ExpeditionRequirements, clearFile))
                {
                    RunWardenEventsFromList(progressionEvent.WardenEvents);
                }
            }
        }

        public static void RunWardenEventsFromList(List<WardenObjectiveEventData> wardenEvents)
        {
            for (int i = 0; i < wardenEvents.Count; i++)
                WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(wardenEvents[i], eWardenObjectiveEventTrigger.None, true);
        }

        public static bool MeetsAllRequirements(List<ExpeditionRequirement> reqs, CM_ExpeditionIcon_New icon, NewClearsFile clearFile)
        {
            if (reqs == null)
                return false;

            bool result = true;
            bool overrideResult = false;
            int highestPriority = -1;
            for (int i = 0; i < reqs.Count; i++)
            {
                ExpeditionRequirement requirement = reqs[i];
                bool allSubRequirementsMet = CheckRequirementsMet(requirement.NeededExpeditionClears, clearFile);
                if (allSubRequirementsMet) 
                {
                    if (requirement.Priority < highestPriority)
                        continue;

                    highestPriority = requirement.Priority;
                    overrideResult = requirement.ForceUnlock;
                    //invisibility
                    icon.IsHidden = requirement.MakeExpeditionInvisible;
                    icon.SetVisible(!requirement.MakeExpeditionInvisible);

                    SetDecryptionStatus(requirement, ref icon);

                    icon.SetDecryptText(requirement.DecryptedText ??= "", requirement.ChangeLockText);
                    //icon.m_decryptErrorText.SetText(requirement.DecryptedText);
                    //component.ChangeText = requirement.ChangeLockText;
                }
                else
                {
                    result = false;
                }
            }

            return overrideResult || result;
        }

        private static void SetDecryptionStatus(ExpeditionRequirement requirement, ref CM_ExpeditionIcon_New icon) 
        {
            switch (requirement.DecryptionState)
            {
                case EDecryptionState.NoDecryption:
                    break;
                case EDecryptionState.SetDecrypted:
                    icon.SetStatus(eExpeditionIconStatus.LockedAndScrambled);
                    icon.Accessibility = eExpeditionAccessibility.BlockedAndScrambled;
                    if (!string.IsNullOrEmpty(requirement.DecryptedText))
                        icon.m_decryptErrorText.SetText(requirement.DecryptedText);
                    break;
                case EDecryptionState.UnlockDecrypted:
                    if (icon.DataBlock.Descriptive.SkipExpNumberInName)
                        icon.SetShortName(icon.DataBlock.Descriptive.Prefix);
                    else
                        icon.SetShortName(icon.DataBlock.Descriptive.Prefix + (icon.ExpIndex + 1).ToString());

                    icon.SetPublicName(icon.DataBlock.Descriptive.PublicName + (icon.DataBlock.Descriptive.IsExtraExpedition ? "<color=orange><size=80%>://EXT</size></color>" : ""));
                    icon.SetFullName(icon.ShortName + " : " + icon.DataBlock.Descriptive.PublicName);
                    icon.SetStatus(eExpeditionIconStatus.TierLockedFinishedAnyway, "-", "-", "-", "-", 1f);
                    icon.Accessibility = eExpeditionAccessibility.AlwayBlock;
                    break;
            }
        }

        public static bool CheckRequirementsMet(List<ExpeditionClearRequirement> requirements, NewClearsFile clearFile)
        {
            for (int i = 0; i < requirements.Count; i++)
            {
                ExpeditionClearRequirement requirement = requirements[i];

                if (!RundownManager.TryGetExpedition(requirement.Expedition.ExpeditionTier, requirement.Expedition.ExpeditionIndex, out ExpeditionInTierData expedition))
                {
                    Logger.Warning("Could not find referenced expedition. Skipping");
                    continue;
                }

                string expeditionName = expedition.Descriptive.PublicName;

                if (!clearFile.ClearData.ContainsKey(expeditionName))
                {
                    Logger.Debug($"Clear data does not contain expedition: {expeditionName}");
                    return false;
                }

                ClearData clearData = clearFile.GetClearDataForExpedition(expeditionName);
                if (!clearData.MatchesRequirement(requirement.NeededClears))
                    return false;
            }

            return true;
        }

        public static void SetSectorSummary(CM_PageRundown_New rundownPage, ClearData allClears, RundownManager.RundownProgData rundownProgression)  
        {
            rundownPage.m_tierMarkerSectorSummary.SetSectorIconTextForMain($"[{allClears.HighClears}/{rundownProgression.totalMain}]", "orange");
            rundownPage.m_tierMarkerSectorSummary.SetSectorIconTextForSecondary($"[{allClears.SecondaryClears}/{rundownProgression.totalSecondary}]", "orange");
            rundownPage.m_tierMarkerSectorSummary.SetSectorIconTextForThird($"[{allClears.OverloadClears}/{rundownProgression.totalThird}]", "orange");
            rundownPage.m_tierMarkerSectorSummary.SetSectorIconTextForAllCleared($"[{allClears.PEClears}/{rundownProgression.totalAllClear}]", "orange");
        }

        public static void SetSectorSummary(CM_PageRundown_New rundownPage, ClearData allClears)
        {
            if (!GetProgressionForRundown(rundownPage, out RundownManager.RundownProgData rundownProgression))
                return;

            rundownPage.m_tierMarkerSectorSummary.SetSectorIconTextForMain($"[{allClears.HighClears}/{rundownProgression.totalMain}]", "orange");
            rundownPage.m_tierMarkerSectorSummary.SetSectorIconTextForSecondary($"[{allClears.SecondaryClears}/{rundownProgression.totalSecondary}]", "orange");
            rundownPage.m_tierMarkerSectorSummary.SetSectorIconTextForThird($"[{allClears.OverloadClears}/{rundownProgression.totalThird}]", "orange");
            rundownPage.m_tierMarkerSectorSummary.SetSectorIconTextForAllCleared($"[{allClears.PEClears}/{rundownProgression.totalAllClear}]", "orange");
        }
    }
}
