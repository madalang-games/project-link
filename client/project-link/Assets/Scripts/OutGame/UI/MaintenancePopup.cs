using ProjectLink.Core;
using TMPro;
using UnityEngine;

namespace ProjectLink.OutGame.UI
{
    public sealed class MaintenancePopup : PopupBase
    {
        [SerializeField] TextMeshProUGUI txtBody;

        public void Init(string message = "")
        {
            txtBody ??= FindText("Txt_Body");
            if (txtBody != null && !string.IsNullOrEmpty(message))
                txtBody.text = message;
        }

        TextMeshProUGUI FindText(string childName)
        {
            foreach (var t in GetComponentsInChildren<TextMeshProUGUI>(true))
                if (t.name == childName) return t;
            return null;
        }

        public override void OnBackPressed() { }
    }
}
