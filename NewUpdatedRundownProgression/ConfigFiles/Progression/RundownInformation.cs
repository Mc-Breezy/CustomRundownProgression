using GameData;

namespace NewUpdatedRundownProgression.ConfigFiles.Progression
{
    public class RundownInformation
    {
        public RundownDataBlock RundownDataBlock { get; set; }
        public string RundownClearPath { get; set; }
        public NewClearsFile ClearData { get; set; }

        public RundownInformation(RundownDataBlock block, string path, NewClearsFile clearData)
        {
            RundownDataBlock = block;
            RundownClearPath = path;
            ClearData = clearData;
        }
    }
}
