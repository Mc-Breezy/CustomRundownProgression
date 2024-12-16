namespace NewUpdatedRundownProgression.ConfigFiles.Progression
{
    public class NewClearsFile
    {
        public ClearData AllClearsInRundown { get; set; }
        public Dictionary<string, ClearData> ClearData { get; set; }

        public ClearData? GetClearDataForExpedition(string expeditionName)
        {
            if (!ClearData.ContainsKey(expeditionName)) 
            {
                Logger.Warning($"Clear data does not contain: {expeditionName}");
                return null;
            }

            return ClearData[expeditionName];
        }

        public NewClearsFile(ClearData allClearsInRundown, Dictionary<string, ClearData> clearData)
        {
            AllClearsInRundown = allClearsInRundown;
            ClearData = clearData;
        }

        public NewClearsFile() 
        {
            AllClearsInRundown = new ClearData();
            ClearData = new Dictionary<string, ClearData>();
        }
    }
}
