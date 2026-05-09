using ProjectLink.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectLink.InGame.UI
{
    public sealed class GameWireframeController : MonoBehaviour
    {
        [SerializeField] Button backButton;
        [SerializeField] Button settingsButton;
        [SerializeField] Button hintButton;
        [SerializeField] Button undoButton;
        [SerializeField] Button paintItemButton;
        [SerializeField] Button hammerButton;
        [SerializeField] Button brushButton;
        [SerializeField] TextMeshProUGUI levelLabelText;
        [SerializeField] TextMeshProUGUI moveCounterText;

        void Start()
        {
            SetText(levelLabelText, $"Stage {GameContext.SelectedStageId}");
            SetText(moveCounterText, "");
        }

        public void SetToolButtonsInteractable(bool interactable)
        {
            if (backButton != null) backButton.interactable = interactable;
            if (settingsButton != null) settingsButton.interactable = interactable;
            if (hintButton != null) hintButton.interactable = interactable;
            if (undoButton != null) undoButton.interactable = interactable;
            if (paintItemButton != null) paintItemButton.interactable = interactable;
            if (hammerButton != null) hammerButton.interactable = interactable;
            if (brushButton != null) brushButton.interactable = interactable;
        }

        static void SetText(TextMeshProUGUI label, string value)
        {
            if (label != null)
                label.text = value ?? "";
        }
    }
}
