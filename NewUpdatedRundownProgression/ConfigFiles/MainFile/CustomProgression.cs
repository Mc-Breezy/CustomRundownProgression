using GameData;
using System.Text.Json.Serialization;

namespace NewUpdatedRundownProgression.ConfigFiles.MainFile
{
    public class CustomProgression
    {
        public string RundownName { get; set; }
        public uint RundownID { get; set; }
        public bool HideTierMarkers { get; set; }

        public RundownTierProgressionData? TierARequirements { get; set; }
        public RundownTierProgressionData? TierBRequirements { get; set; }
        public RundownTierProgressionData? TierCRequirements { get; set; }
        public RundownTierProgressionData? TierDRequirements { get; set; }
        public RundownTierProgressionData? TierERequirements { get; set; }

        public List<CustomTierRequirement> CustomTierRequirements { get; set; }

        [JsonIgnore]
        private Dictionary<eRundownTier, List<CustomTierRequirement>> _entriesPerTier;

        public void ParseEntriesToDictionary() 
        {
            foreach (var entry in CustomTierRequirements)
            {
                if (!_entriesPerTier.ContainsKey(entry.Expedition.ExpeditionTier))
                {
                    _entriesPerTier.Add(entry.Expedition.ExpeditionTier, new List<CustomTierRequirement>());
                    _entriesPerTier[entry.Expedition.ExpeditionTier].Add(entry);
                }
                else
                {
                    _entriesPerTier[entry.Expedition.ExpeditionTier].Add(entry);
                }
            }
        }

        public CustomTierRequirement? GetCustomEntry(eRundownTier tier, int expeditionIndex) 
        {
            if (!_entriesPerTier.ContainsKey(tier))
                return null;
            else
                return _entriesPerTier[tier][expeditionIndex];
        }

        public RundownTierProgressionData? GetTierProgressionData(eRundownTier tier) 
        {
            return tier switch
            {
                eRundownTier.TierA => TierARequirements,
                eRundownTier.TierB => TierBRequirements,
                eRundownTier.TierC => TierCRequirements,
                eRundownTier.TierD => TierDRequirements,
                eRundownTier.TierE => TierERequirements,
                _ => null
            };
        }

        public CustomProgression()
        {
            RundownName = "INTERNAL_NAME";
            RundownID = 0;
            HideTierMarkers = false;
            CustomTierRequirements = new List<CustomTierRequirement>()
            {
                new CustomTierRequirement()
            };
            _entriesPerTier = new();
        }
    }
}
