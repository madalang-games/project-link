using ProjectLink.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ProjectLink.Core;

namespace ProjectLink.OutGame.UI
{
    public sealed class BootstrapWireframeController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI loadingLabelText;
        [SerializeField] TextMeshProUGUI versionText;
        [SerializeField] Image progressFillImage;
        [SerializeField] Button retryButton;
        [SerializeField] TextMeshProUGUI networkErrorText;

        BootstrapViewModel _viewModel;
        bool _forceUpdateShown;
        bool _titleLoadRequested;

        void Start()
        {
            retryButton ??= FindButton("Btn_Retry");
            networkErrorText ??= FindText("Txt_NetworkError");
            if (retryButton != null)
            {
                retryButton.onClick.RemoveAllListeners();
                retryButton.onClick.AddListener(Retry);
            }

            _viewModel = new BootstrapViewModel(UiServiceLocator.UiData);
            _viewModel.Changed += Render;
            _viewModel.Load();
        }

        void OnDestroy()
        {
            if (_viewModel != null)
                _viewModel.Changed -= Render;
        }

        void Retry()
        {
            _viewModel?.Load();
        }

        void Render()
        {
            if (_viewModel == null) return;

            SetText(loadingLabelText, LocalizationManager.Get(_viewModel.StatusStringId));
            SetText(versionText, _viewModel.Version);
            SetText(networkErrorText, LocalizationManager.GetError(_viewModel.ErrorCode));
            if (networkErrorText != null)
                networkErrorText.gameObject.SetActive(!string.IsNullOrEmpty(_viewModel.ErrorCode));
            if (retryButton != null)
                retryButton.gameObject.SetActive(_viewModel.RetryVisible);
            if (progressFillImage != null)
                progressFillImage.fillAmount = _viewModel.Progress;

            if (_viewModel.RequiresForceUpdate)
            {
                if (!_forceUpdateShown)
                {
                    _forceUpdateShown = true;
                    PopupManager.Request(PopupId.ForceUpdate);
                }
                return;
            }

            if (_viewModel.ReadyToEnterTitle && SceneLoader.Instance != null && !_titleLoadRequested)
            {
                _titleLoadRequested = true;
                SceneLoader.Instance.LoadScene("Title");
            }
        }

        static void SetText(TextMeshProUGUI label, string value)
        {
            if (label != null)
                label.text = value ?? "";
        }

        Button FindButton(string childName)
        {
            foreach (var button in GetComponentsInChildren<Button>(true))
                if (button.name == childName) return button;
            return null;
        }

        TextMeshProUGUI FindText(string childName)
        {
            foreach (var label in GetComponentsInChildren<TextMeshProUGUI>(true))
                if (label.name == childName) return label;
            return null;
        }
    }
}
