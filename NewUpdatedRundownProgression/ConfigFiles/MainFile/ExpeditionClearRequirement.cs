namespace NewUpdatedRundownProgression.ConfigFiles.MainFile
{
    public class ExpeditionClearRequirement
    {
        public ExpIndex Expedition { get; set; }
        public ClearData NeededClears { get; set; } 

        public ExpeditionClearRequirement() 
        {
            Expedition = new ExpIndex();
            NeededClears = new ClearData(); 
        }
    }
}
