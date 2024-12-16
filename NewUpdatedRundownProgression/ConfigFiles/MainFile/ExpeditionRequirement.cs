namespace NewUpdatedRundownProgression.ConfigFiles.MainFile
{
    public enum EDecryptionState
    {
        NoDecryption,
        SetDecrypted,
        UnlockDecrypted
    }

    public class ExpeditionRequirement
    {
        public List<ExpeditionClearRequirement> NeededExpeditionClears { get; set; }
        public bool MakeExpeditionInvisible { get; set; }
        public EDecryptionState DecryptionState { get; set; }
        public bool ChangeLockText { get; set; }
        public string DecryptedText { get; set; }
        public bool ForceUnlock { get; set; }
        public int Priority { get; set; }

        public ExpeditionRequirement()
        {
            NeededExpeditionClears = new();
            MakeExpeditionInvisible = false;
            DecryptionState = EDecryptionState.NoDecryption;
            ChangeLockText = false;
            DecryptedText = string.Empty;
            ForceUnlock = false;
            Priority = -1;
            NeededExpeditionClears = new List<ExpeditionClearRequirement>() { new ExpeditionClearRequirement() };
        }
    }
}
