using ProjectLink.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectLink.OutGame.UI
{
    public sealed class ForceUpdatePopup : PopupBase
    {
        [SerializeField] Button btnOpenStore;

        public void Init()
        {
            btnOpenStore ??= FindButton("Btn_OpenStore");
            if (btnOpenStore != null)
                btnOpenStore.onClick.AddListener(OnOpenStore);
        }

        void OnOpenStore()
        {
#if UNITY_ANDROID
            Application.OpenURL($"market://details?id={Application.identifier}");
#elif UNITY_IOS
            Application.OpenURL($"itms-apps://itunes.apple.com/app/id{Application.identifier}");
#else
            Application.OpenURL("https://example.com");
#endif
        }

        Button FindButton(string childName)
        {
            foreach (var b in GetComponentsInChildren<Button>(true))
                if (b.name == childName) return b;
            return null;
        }

        public override void OnBackPressed() { }
    }
}
