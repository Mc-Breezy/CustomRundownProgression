using MTFO.Managers;
using NewUpdatedRundownProgression.ConfigFiles.MainFile;
using System.Text.Json;

namespace NewUpdatedRundownProgression.PluginInfo
{
#nullable disable

    internal class RundownProgressionFileSetup
    {
        public const string Name = "RundownProgression.json";
        public static string FilePath { get; } = Path.Combine(ConfigManager.CustomPath, Name);

        private static List<CustomProgression> s_progressionFiles = new();

        public static CustomProgression GetCustomProgressionFile(string rundownName) 
        {
            if (s_progressionFiles.Count == 0)
            {
                Logger.Warning("There were no custom progression files detected");
                return null;
            }

            for (int i = 0; i < s_progressionFiles.Count; i++)
                if (s_progressionFiles[i].RundownName.Equals(rundownName))
                    return s_progressionFiles[i];

            Logger.Warning($"Could not find progression file for name: {rundownName}");
            return null;
        }

        public static CustomProgression GetCustomProgressionFile(uint rundownID) 
        {
            if (s_progressionFiles.Count == 0)
            {
                Logger.Warning("There were no custom progression files detected");
                return null;
            }

            for (int i = 0; i < s_progressionFiles.Count; i++)
                if (s_progressionFiles[i].RundownID.Equals(rundownID))
                    return s_progressionFiles[i];

            Logger.Warning($"Could not find progression file for ID: {rundownID}");
            return null;
        }

        public static void Load() 
        {
            if (File.Exists(FilePath)) 
            {
                s_progressionFiles = JsonSerializer.Deserialize<List<CustomProgression>>(File.ReadAllText(FilePath), EntryPoint.SerializerOptions);
                Logger.Debug(Name + " has loaded successfully");

            }
            else
            {
                Logger.Warning("Could not find custom progression file");
                s_progressionFiles = new List<CustomProgression>()
                {
                    new CustomProgression()
                };
                string jsonOutput = JsonSerializer.Serialize(s_progressionFiles, EntryPoint.SerializerOptions);
                File.WriteAllText(FilePath, jsonOutput);
            }

            s_progressionFiles.ForEach(x => x.ParseEntriesToDictionary());
        }
    }
}
