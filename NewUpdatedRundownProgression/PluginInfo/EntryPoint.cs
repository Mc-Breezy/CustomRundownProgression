using BepInEx;
using BepInEx.Unity.IL2CPP;
using GTFO.API;
using GTFO.API.JSON.Converters;
using HarmonyLib;
using System.Text.Json;

#nullable disable

namespace NewUpdatedRundownProgression.PluginInfo
{
    [BepInPlugin("com.Breeze.UpdatedExtraSettings", "UpdatedExtraSettings", "0.0.2")]
    [BepInDependency("GTFO.InjectLib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("GTFO.exe")]
    internal class EntryPoint : BasePlugin
    {
        const string GUID = "UpdatedExtraSettings";
        private Harmony _harmony;
        public static readonly JsonSerializerOptions SerializerOptions = new() 
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = true,
            IncludeFields = true,
            AllowTrailingCommas = true,
            WriteIndented = true,
            Converters = 
            {
                new Vector3Converter(),
                new LocalizedTextConverter(),
            }
        };
        public override void Load()
        {
            Logger.SetLogSource(Log);
            _harmony = new Harmony(GUID);
            _harmony.PatchAll();
            RundownProgressionFileSetup.Load();
            LevelAPI.OnEnterLevel += OnLevelEnter.EnterLevel;
        }
    }
}
