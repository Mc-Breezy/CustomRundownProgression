using GameData;

namespace NewUpdatedRundownProgression.ConfigFiles.MainFile
{
    public class ProgressionWardenEvent
    {
        public List<ExpeditionClearRequirement> ExpeditionRequirements { get; set; }
        public List<WardenObjectiveEventData> WardenEvents {  get; set; }

        public ProgressionWardenEvent() 
        {
            ExpeditionRequirements = new List<ExpeditionClearRequirement>() 
            {
                new ExpeditionClearRequirement()
            };
            WardenEvents = new List<WardenObjectiveEventData>();
        }
    }
}
