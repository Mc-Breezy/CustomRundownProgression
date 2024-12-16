using UnityEngine;

namespace NewUpdatedRundownProgression.ConfigFiles.MainFile
{
    public class CustomTierRequirement
    {
        public ExpIndex Expedition { get; set; }

        public bool ChangePosition { get; set; }
        public Vector3 NewPosition { get; set; }

        public ExpeditionLockData LockData { get; set; }
        public List<ProgressionWardenEvent> WardenEventsOnLand { get; set; }

        public CustomTierRequirement() 
        {
            Expedition = new ExpIndex();
            NewPosition = new Vector3();
            LockData = new ExpeditionLockData();
            WardenEventsOnLand = new List<ProgressionWardenEvent>() 
            {
                new ProgressionWardenEvent()
            };
        }
    }
}
