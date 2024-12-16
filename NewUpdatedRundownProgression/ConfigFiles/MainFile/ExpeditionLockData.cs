namespace NewUpdatedRundownProgression.ConfigFiles.MainFile
{
    public enum ELockType
    {
        Default,
        UnlockedByTierClears,
        UnlockedByOtherExpeditions
    }

    public class ExpeditionLockData
    {
        public bool HideExpedition { get; set; }
        public ELockType LockType { get; set; }
        public List<ExpeditionRequirement> Requirements { get; set; }

        public ExpeditionLockData() 
        {
            HideExpedition = false;
            LockType = ELockType.Default;
            Requirements = new() 
            {
                new()
            };
        } 
    }
}
