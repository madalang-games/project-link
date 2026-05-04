// Moved to ProjectLink.Core.HapticManager
// Shim kept so existing Unity meta files and callers in this namespace remain valid
using ProjectLink.Core;

namespace ProjectLink.InGame.UI
{
    public static class HapticManager
    {
        public static bool Enabled
        {
            get => Core.HapticManager.Enabled;
            set => Core.HapticManager.Enabled = value;
        }

        public static void PlayBlocked()   => Core.HapticManager.PlayBlocked();
        public static void PlayConnected() => Core.HapticManager.PlayConnected();
        public static void PlayErased()    => Core.HapticManager.PlayErased();
    }
}
