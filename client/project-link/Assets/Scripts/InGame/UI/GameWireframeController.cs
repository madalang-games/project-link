using ProjectLink.Core;
using ProjectLink.OutGame.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectLink.InGame.UI
{
    public sealed class GameWireframeController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI levelLabelText;
        [SerializeField] TextMeshProUGUI pipeCounterText;
        [SerializeField] TextMeshProUGUI moveCounterText;
        [SerializeField] TextMeshProUGUI timerText;

        [Header("Item Toolbar")]
        [SerializeField] UnityEngine.UI.Button item1Button;
        [SerializeField] UnityEngine.UI.Button item2Button;
        [SerializeField] UnityEngine.UI.Button item3Button;
        [SerializeField] UnityEngine.UI.Button item4Button;
        [SerializeField] TextMeshProUGUI item1CountText;
        [SerializeField] TextMeshProUGUI item2CountText;
        [SerializeField] TextMeshProUGUI item3CountText;
        [SerializeField] TextMeshProUGUI item4CountText;

        int _stageId;

        public TextMeshProUGUI PipeCounterText => pipeCounterText;
        public TextMeshProUGUI MoveCounterText => moveCounterText;
        public TextMeshProUGUI TimerText => timerText;

        public UnityEngine.UI.Button Item1Button => item1Button;
        public UnityEngine.UI.Button Item2Button => item2Button;
        public UnityEngine.UI.Button Item3Button => item3Button;
        public UnityEngine.UI.Button Item4Button => item4Button;
        public TextMeshProUGUI Item1CountText => item1CountText;
        public TextMeshProUGUI Item2CountText => item2CountText;
        public TextMeshProUGUI Item3CountText => item3CountText;
        public TextMeshProUGUI Item4CountText => item4CountText;

        void Awake()
        {
            timerText ??= FindText("Txt_Timer");
            if (moveCounterText == null)
                moveCounterText = FindText("Txt_Moves");
            pipeCounterText ??= FindText("Txt_Pipe");
            ResolveItemSlots();
            EnsurePipeCounterText();
        }

        void ResolveItemSlots()
        {
            var toolbar = FindRect("Toolbar_Items");
            if (toolbar == null) return;
            for (int i = 0; i < 4; i++)
            {
                var slot = toolbar.Find($"ItemSlot_{i + 1}");
                if (slot == null) continue;
                var btn = slot.GetComponent<UnityEngine.UI.Button>();
                var txt = slot.Find("Txt_Count")?.GetComponent<TextMeshProUGUI>();
                switch (i)
                {
                    case 0: item1Button ??= btn; item1CountText ??= txt; break;
                    case 1: item2Button ??= btn; item2CountText ??= txt; break;
                    case 2: item3Button ??= btn; item3CountText ??= txt; break;
                    case 3: item4Button ??= btn; item4CountText ??= txt; break;
                }
            }
        }

        void Start()
        {
            SetStageLabel(GameContext.SelectedStageId);
            AddPressEffects();
        }

        void EnsurePipeCounterText()
        {
            if (pipeCounterText != null) return;
            if (moveCounterText == null) return;

            var parent = moveCounterText.transform.parent;
            var go = new GameObject("Txt_Pipe", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            go.transform.SetSiblingIndex(moveCounterText.transform.GetSiblingIndex());

            go.GetComponent<LayoutElement>().flexibleWidth = 1;

            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.fontSize = moveCounterText.fontSize;
            tmp.fontStyle = moveCounterText.fontStyle;
            tmp.color = moveCounterText.color;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.raycastTarget = false;
            tmp.text = "0 / 0";

            pipeCounterText = tmp;
            go.AddComponent<LocalizedFont>();
        }

        void AddPressEffects()
        {
            EnsurePressEffect(item1Button);
            EnsurePressEffect(item2Button);
            EnsurePressEffect(item3Button);
            EnsurePressEffect(item4Button);

            foreach (var btn in GetComponentsInChildren<Button>(true))
            {
                if (btn.name == "Btn_Pause") { EnsurePressEffect(btn); break; }
            }
        }

        static void EnsurePressEffect(Button btn)
        {
            if (btn == null) return;
            if (btn.GetComponent<ButtonPressEffect>() == null)
                btn.gameObject.AddComponent<ButtonPressEffect>();
        }

        void OnEnable()  { LocalizationManager.LanguageChanged += OnLanguageChanged; }
        void OnDisable() { LocalizationManager.LanguageChanged -= OnLanguageChanged; }
        void OnDestroy() { LocalizationManager.LanguageChanged -= OnLanguageChanged; }

        void OnLanguageChanged() => SetStageLabel(_stageId);

        public void SetStageLabel(int stageId)
        {
            _stageId = stageId;
            SetText(levelLabelText, string.Format(LocalizationManager.Get("popup.stage.title_n_fmt"), stageId));
        }

        TextMeshProUGUI FindText(string childName)
        {
            foreach (var t in GetComponentsInChildren<TextMeshProUGUI>(true))
                if (t.name == childName) return t;
            return null;
        }

        RectTransform FindRect(string childName)
        {
            foreach (var r in GetComponentsInChildren<RectTransform>(true))
                if (r.name == childName) return r;
            return null;
        }

        static void SetText(TextMeshProUGUI label, string value)
        {
            if (label != null)
                label.text = value ?? "";
        }
    }
}
