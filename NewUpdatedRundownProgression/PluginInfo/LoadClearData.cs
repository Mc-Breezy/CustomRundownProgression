using GameData;
using NewUpdatedRundownProgression.ConfigFiles;
using NewUpdatedRundownProgression.ConfigFiles.Progression;
using System.Text.Json;

namespace NewUpdatedRundownProgression.PluginInfo
{
    internal class LoadClearData
    {
        public static string ProgressionDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GTFO-Modding", "MCProgression_Rundowns");

        private static Dictionary<uint, RundownInformation> s_allRundownInformation = new();

        public static NewClearsFile? GetClearData(uint rundownID)
            => CheckRundownEntry(rundownID) ? 
            s_allRundownInformation[rundownID].ClearData : null;

        public static string? GetClearPath(uint rundownID) 
            => CheckRundownEntry(rundownID) ? s_allRundownInformation[rundownID].RundownClearPath : null;

        private static bool CheckRundownEntry(uint valueToCheck) 
            => s_allRundownInformation.Count > 0 && s_allRundownInformation.ContainsKey(valueToCheck);

        public static void Load() 
        {
            if (!Directory.Exists(ProgressionDirectory))
                Directory.CreateDirectory(ProgressionDirectory);

            s_allRundownInformation.Clear();

            GameSetupDataBlock gameSetup = GameDataBlockBase<GameSetupDataBlock>.GetBlock(1);

            int rundownCount = gameSetup.RundownIdsToLoad.Count;
            for (int i = 0; i < rundownCount; i++) 
            {
                RundownDataBlock rundownBlock = GameDataBlockBase<RundownDataBlock>.GetBlock(gameSetup.RundownIdsToLoad[i]);
                string fileName = $"Clears_{rundownBlock.name}.json";
                string path = Path.Combine(ProgressionDirectory, fileName);
                NewClearsFile clears;

                if (File.Exists(path))
                {
                    clears = ReadClearData(File.ReadAllText(path), path, rundownBlock);
                    Logger.Debug($"Found clear data for Rundown: {rundownBlock.name}");
                }
                else 
                {
                    clears = new NewClearsFile();
                    string jsonOutput = JsonSerializer.Serialize(clears, EntryPoint.SerializerOptions);
                    File.WriteAllText(path, jsonOutput);
                    Logger.Warning($"Created clear file for Rundown: {rundownBlock.name}");
                }

                s_allRundownInformation.Add(rundownBlock.persistentID, new RundownInformation(rundownBlock, path, clears));
            }
        }

        private static NewClearsFile? ReadClearData(string jsonContent, string path, RundownDataBlock block) 
        {
            if (jsonContent.Contains("TierAClearData"))
            {
                NewClearsFile newClears = ConvertOldFile(jsonContent, block);
                JsonSerializer.Serialize(newClears, EntryPoint.SerializerOptions);
                File.WriteAllText(path, jsonContent);
                return newClears;
            }
            else
                return JsonSerializer.Deserialize<NewClearsFile>(jsonContent, EntryPoint.SerializerOptions);
        }

        private static NewClearsFile? ConvertOldFile(string content, RundownDataBlock rundown) 
        {
            OldClearJsonFile oldFile = JsonSerializer.Deserialize<OldClearJsonFile>(content, EntryPoint.SerializerOptions);
            if (oldFile == null)
            {
                Logger.Error("Old file was null?");
                return null;
            }

            Dictionary<string, ClearData> clearData = new();

            Dictionary<eRundownTier, List<ClearData>> dataToParse = new()
            {
                { eRundownTier.TierA, oldFile.TierAClearData },
                { eRundownTier.TierB, oldFile.TierBClearData },
                { eRundownTier.TierC, oldFile.TierCClearData },
                { eRundownTier.TierD, oldFile.TierDClearData },
                { eRundownTier.TierE, oldFile.TierEClearData }
            };

            foreach (var item in dataToParse)
            {
                ParseClearTierData(item.Value, rundown, item.Key).ToList().ForEach(x => clearData.Add(x.Key, x.Value));
            }

            oldFile.AllClearsInRundown ??= new ClearData();

            return new NewClearsFile(oldFile.AllClearsInRundown, clearData);
        }

        private static Dictionary<string, ClearData> ParseClearTierData(List<ClearData> dataToParse, RundownDataBlock block, eRundownTier tier) 
        {
            if (dataToParse == null)
                return new();

            Dictionary<string, ClearData> result = new();
            Il2CppSystem.Collections.Generic.List<ExpeditionInTierData> selectedTier = tier switch
            {
                eRundownTier.TierA => block.TierA,
                eRundownTier.TierB => block.TierB,
                eRundownTier.TierC => block.TierC,
                eRundownTier.TierD => block.TierD,
                eRundownTier.TierE => block.TierE,
                _ => null
            };

            if (selectedTier == null) 
            {
                Logger.Warning("Selected tier data was null");
                return result;
            }

            for (int i = 0; i < selectedTier.Count; i++)
            {
                if (dataToParse[i].HighClears == 0)
                    continue;

                result.Add(selectedTier[i].Descriptive.PublicName, dataToParse[i]);
            }

            return result;
        }
    }
}
