using CellMenu;

namespace NewUpdatedRundownProgression.Patches
{
    public static class IconExtension
    {
        private static Dictionary<string, (string decryptText, bool textEnabled)> s_iconStates = new();

        public static void LocalUpdate(this CM_ExpeditionIcon_New icon) 
        {
            if (!icon.GetIconInformation(out var information))
                return;

            icon.m_decryptErrorText.SetText(information.text);
            icon.m_decryptErrorText.gameObject.SetActive(information.enabled);
        }

        public static bool GetIconInformation(this CM_ExpeditionIcon_New icon, out (string text, bool enabled) info) 
        {
            if (!s_iconStates.ContainsKey(icon.DataBlock.Descriptive.PublicName))
            {
                info = default;
                return false;
            }

            info = s_iconStates[icon.DataBlock.Descriptive.PublicName];
            return true;
        }

        public static void SetDecryptText(this CM_ExpeditionIcon_New icon, string text, bool enabled) 
        {
            icon.m_decryptErrorText.SetText(text);
            icon.m_decryptErrorText.gameObject.SetActive(enabled);
            icon.AddInformation(text, enabled);
        }

        public static void AddInformation(this CM_ExpeditionIcon_New icon, string text, bool enabled) 
        {
            if (s_iconStates.ContainsKey(icon.DataBlock.Descriptive.PublicName))
                return;

            s_iconStates.Add(icon.DataBlock.Descriptive.PublicName, new(text, enabled));
        }

        public static void Cleanup() => s_iconStates.Clear(); //Possibly needed but most likely not
    }
}
