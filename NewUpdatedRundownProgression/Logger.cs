using BepInEx.Logging;

namespace NewUpdatedRundownProgression
{
    public static class Logger
    {
        static ManualLogSource? s_logSource;

        public static void SetLogSource(ManualLogSource? logSource) => s_logSource = logSource;

        public static void Debug(string text) => s_logSource.LogDebug(text);
        public static void Warning(string text) => s_logSource.LogWarning(text);
        public static void Error(string text) => s_logSource.LogError(text);
    }
}
