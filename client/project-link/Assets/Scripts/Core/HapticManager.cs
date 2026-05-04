using UnityEngine;

namespace ProjectLink.Core
{
    public class HapticManager : MonoBehaviour
    {
        public static HapticManager Instance { get; private set; }

        bool _enabled = true;

        public static bool Enabled
        {
            get => Instance != null && Instance._enabled;
            set { if (Instance != null) Instance._enabled = value; }
        }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if (DataManager.Instance != null)
                _enabled = DataManager.Instance.HapticEnabled;
        }

        public static void PlayBlocked()   => Instance?.TryVibrate();
        public static void PlayConnected() => Instance?.TryVibrate();
        public static void PlayErased()    => Instance?.TryVibrate();

        void TryVibrate()
        {
            if (!_enabled) return;
#if UNITY_ANDROID || UNITY_IOS
            try { Handheld.Vibrate(); }
            catch { }
#endif
        }
    }
}
