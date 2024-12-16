using GameData;

namespace NewUpdatedRundownProgression.ConfigFiles
{
    public class ClearData
    {
        public int HighClears { get; set; }
        public int SecondaryClears {  get; set; }
        public int OverloadClears { get; set; }
        public int PEClears { get; set; }

        public bool MatchesRequirement(ClearData comparingTo) 
        {
            if (HighClears < comparingTo.HighClears)
                return false;
            if (SecondaryClears < comparingTo.SecondaryClears)
                return false;
            if (OverloadClears < comparingTo.OverloadClears)
                return false;
            if (PEClears < comparingTo.PEClears)
                return false;

            return true;
        }

        public bool CheckTierRequirement(RundownTierProgressionData tierData)
        {
            if (HighClears < tierData.MainSectors)
                return false;
            if (SecondaryClears < tierData.SecondarySectors)
                return false;
            if (OverloadClears < tierData.ThirdSectors)
                return false;
            if (PEClears < tierData.AllClearedSectors)
                return false;

            return true;
        }

        public ClearData() 
        {
            HighClears = 0;
            SecondaryClears = 0;
            OverloadClears = 0;
            PEClears = 0;
        }
    }
}
