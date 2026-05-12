using ProjectLink.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectLink.OutGame.UI
{
    public sealed class StageDetailPopup : PopupBase
    {
        [SerializeField] Button btnClose;
        [SerializeField] Button btnPlay;
        [SerializeField] TextMeshProUGUI txtBest;
        [SerializeField] TextMeshProUGUI txtMyRank;

        public void Init(int stageId = 0)
        {
            btnClose ??= FindButton("Btn_Close");
            btnPlay ??= FindButton("Btn_Play");
            txtBest ??= FindText("Txt_Best");
            txtMyRank ??= FindText("Txt_MyRank");

            if (btnClose != null)
                btnClose.onClick.AddListener(() => PopupManager.Instance.CloseTop());
            if (btnPlay != null)
                btnPlay.onClick.AddListener(OnPlay);
        }

        void OnPlay()
        {
            PopupManager.Instance.CloseTop();
            SceneLoader.Instance.LoadScene("Game");
        }

        Button FindButton(string childName)
        {
            foreach (var b in GetComponentsInChildren<Button>(true))
                if (b.name == childName) return b;
            return null;
        }

        TextMeshProUGUI FindText(string childName)
        {
            foreach (var t in GetComponentsInChildren<TextMeshProUGUI>(true))
                if (t.name == childName) return t;
            return null;
        }
    }
}
