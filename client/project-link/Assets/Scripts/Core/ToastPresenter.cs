using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectLink.Core
{
    public sealed class ToastPresenter : MonoBehaviour
    {
        const int MaxToasts = 3;
        const float LifetimeSeconds = 3f;

        readonly Queue<GameObject> _toasts = new();
        RectTransform _root;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            UiEventBus.Subscribe<UiErrorRaised>(OnError);
        }

        void OnDisable()
        {
            UiEventBus.Unsubscribe<UiErrorRaised>(OnError);
        }

        void OnError(UiErrorRaised error)
        {
            if (error.Blocking || string.IsNullOrEmpty(error.ErrorCode))
                return;

            var localized = LocalizationManager.GetError(error.ErrorCode);
            Show(localized == error.ErrorCode && !string.IsNullOrEmpty(error.Message)
                ? error.Message
                : localized);
        }

        void Show(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            EnsureRoot();
            while (_toasts.Count >= MaxToasts)
                Destroy(_toasts.Dequeue());

            var toast = CreateToast(message);
            _toasts.Enqueue(toast);
            Destroy(toast, LifetimeSeconds);
        }

        void EnsureRoot()
        {
            if (_root != null)
                return;

            var parent = UIManager.Instance != null
                ? UIManager.Instance.GetLayer(UILayer.System)
                : transform;

            var go = new GameObject("ToastStack", typeof(RectTransform), typeof(VerticalLayoutGroup));
            go.transform.SetParent(parent, false);
            _root = go.GetComponent<RectTransform>();
            _root.anchorMin = new Vector2(0f, 1f);
            _root.anchorMax = new Vector2(1f, 1f);
            _root.pivot = new Vector2(0.5f, 1f);
            _root.offsetMin = new Vector2(64f, _root.offsetMin.y);
            _root.offsetMax = new Vector2(-64f, _root.offsetMax.y);
            _root.sizeDelta = new Vector2(_root.sizeDelta.x, 360f);
            _root.anchoredPosition = new Vector2(0f, -96f);

            var layout = go.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 12f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
        }

        GameObject CreateToast(string message)
        {
            var go = new GameObject("Toast", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            go.transform.SetParent(_root, false);

            var image = go.GetComponent<Image>();
            image.color = new Color(0.05f, 0.07f, 0.12f, 0.92f);

            var element = go.GetComponent<LayoutElement>();
            element.preferredHeight = 88f;

            var label = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            label.transform.SetParent(go.transform, false);
            var rect = label.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(28f, 12f);
            rect.offsetMax = new Vector2(-28f, -12f);

            var tmp = label.GetComponent<TextMeshProUGUI>();
            tmp.text = message;
            tmp.fontSize = 28f;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 18f;
            tmp.fontSizeMax = 28f;
            tmp.raycastTarget = false;
            ApplyFont(tmp);

            return go;
        }

        static void ApplyFont(TextMeshProUGUI label)
        {
            var registry = FontRegistry.Instance;
            if (registry == null || LocalizationManager.Instance == null)
                return;

            bool isBold = (label.fontStyle & FontStyles.Bold) != 0;
            var lang = LocalizationManager.Instance.CurrentLanguage;
            if (registry.TryGetFonts(lang, out var regular, out var bold))
                label.font = isBold ? bold ?? regular ?? label.font : regular ?? bold ?? label.font;
        }
    }
}
