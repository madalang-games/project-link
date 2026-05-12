using System.Globalization;
using ProjectLink.Core;
using ProjectLink.Contracts.Ranking;
using ProjectLink.Services;
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
        [SerializeField] RectTransform starRow;
        [SerializeField] RectTransform rankContent;

        int _stageId;
        bool _initialized;
        RectTransform _myRankPanel;

        public void Init(int stageId = 0)
        {
            if (_initialized) return;
            _initialized = true;
            _stageId = Mathf.Max(1, stageId > 0 ? stageId : GameContext.SelectedStageId);
            btnClose ??= FindButton("Btn_Close");
            btnPlay ??= FindButton("Btn_Play");
            txtBest ??= FindText("Txt_Best");
            txtMyRank ??= FindText("Txt_MyRank");
            starRow ??= FindRect("Group_Stars");
            rankContent ??= FindRect("RankContent");
            EnsureRankingContent();
            BindOverlayClose();

            if (btnClose != null)
                btnClose.onClick.AddListener(() => PopupManager.Instance.CloseTop());
            if (btnPlay != null)
                btnPlay.onClick.AddListener(OnPlay);

            SetText(txtBest, $"Stage {_stageId}");
            SetText(txtMyRank, "My Ranking");
            UiServiceLocator.UiData.GetProgress(ApplyProgress);
            UiServiceLocator.UiData.GetRanking($"stage:{_stageId}", ApplyRanking);
        }

        void OnPlay()
        {
            PopupManager.Instance.CloseTop();
            RuntimeNavigationButtons.EnterStage(_stageId);
        }

        void ApplyProgress(ServiceResult<ProjectLink.Contracts.Progress.ProgressResponse> result)
        {
            int stars = 0;
            if (result.IsSuccess && result.Value != null)
            {
                for (int i = 0; i < result.Value.Stages.Count; i++)
                {
                    var entry = result.Value.Stages[i];
                    if (entry.StageId != _stageId) continue;
                    stars = Mathf.Clamp(entry.Stars, 0, 3);
                    break;
                }
            }

            ClearChildren(starRow);
            for (int i = 0; i < 3; i++)
                AddStar(i < stars);
            SetText(txtBest, $"Stage {_stageId}   {stars}/3");
        }

        void ApplyRanking(ServiceResult<RankingListResponse> result)
        {
            ClearChildren(rankContent);
            ClearChildren(_myRankPanel);

            if (!result.IsSuccess || result.Value == null)
            {
                AddMyRankRow(null, result.ErrorCode);
                return;
            }

            var ranking = result.Value;

            int count = Mathf.Min(10, ranking.Entries.Count);
            for (int i = 0; i < count; i++)
            {
                var entry = ranking.Entries[i];
                AddRankRow($"#{entry.Rank}", string.IsNullOrEmpty(entry.DisplayName) ? "Guest" : entry.DisplayName,
                    FormatScore(entry.Value), entry.IsMe);
            }

            AddMyRankRow(ranking.MyRank, null);
        }

        void AddMyRankRow(RankingEntry myRank, string errorText)
        {
            if (_myRankPanel == null) return;

            string rankStr, scoreStr;
            bool hasRank;

            if (!string.IsNullOrEmpty(errorText))
            {
                rankStr = errorText;
                scoreStr = "";
                hasRank = false;
            }
            else if (myRank != null)
            {
                rankStr = $"#{myRank.Rank}";
                scoreStr = FormatScore(myRank.Value);
                hasRank = true;
            }
            else
            {
                rankStr = "-";
                scoreStr = "-";
                hasRank = false;
            }

            var row = new GameObject("MyRankRow", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            row.transform.SetParent(_myRankPanel, false);
            row.GetComponent<Image>().color = hasRank
                ? new Color(0.12f, 0.45f, 0.9f, 0.85f)
                : new Color(1f, 1f, 1f, 0.06f);
            row.GetComponent<LayoutElement>().preferredHeight = 60f;
            var layout = row.GetComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 8, 8);
            layout.spacing = 12f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;

            AddLabel(row.transform, rankStr, 0.35f, TextAlignmentOptions.MidlineLeft);
            if (!string.IsNullOrEmpty(scoreStr))
                AddLabel(row.transform, scoreStr, 1f, TextAlignmentOptions.MidlineRight);
        }

        void EnsureRankingContent()
        {
            if (rankContent != null || txtMyRank == null)
                return;

            var parent = txtMyRank.transform.parent as RectTransform;
            if (parent == null)
                return;

            var scroll = new GameObject("RankScroll", typeof(RectTransform), typeof(Image), typeof(Mask), typeof(ScrollRect), typeof(LayoutElement));
            scroll.transform.SetParent(parent, false);
            scroll.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.06f);
            scroll.GetComponent<Mask>().showMaskGraphic = false;
            scroll.GetComponent<LayoutElement>().preferredHeight = 320f;

            var content = new GameObject("RankContent", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(scroll.transform, false);
            rankContent = content.GetComponent<RectTransform>();
            rankContent.anchorMin = new Vector2(0f, 1f);
            rankContent.anchorMax = new Vector2(1f, 1f);
            rankContent.pivot = new Vector2(0.5f, 1f);
            rankContent.offsetMin = Vector2.zero;
            rankContent.offsetMax = Vector2.zero;

            var layout = content.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 12, 12);
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;

            var fitter = content.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scrollRect = scroll.GetComponent<ScrollRect>();
            scrollRect.viewport = scroll.GetComponent<RectTransform>();
            scrollRect.content = rankContent;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.horizontal = false;

            // MyRank panel below the scroll
            var myRankContainer = new GameObject("MyRankPanel", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            myRankContainer.transform.SetParent(parent, false);
            myRankContainer.GetComponent<LayoutElement>().minHeight = 64f;
            var myLayout = myRankContainer.GetComponent<VerticalLayoutGroup>();
            myLayout.padding = new RectOffset(0, 0, 4, 0);
            myLayout.childControlWidth = true;
            myLayout.childControlHeight = true;
            _myRankPanel = myRankContainer.GetComponent<RectTransform>();
        }

        void AddStar(bool filled)
        {
            if (starRow == null) return;

            var go = new GameObject("Star", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            go.transform.SetParent(starRow, false);
            go.GetComponent<Image>().color = filled ? new Color(1f, 0.82f, 0.15f, 1f) : new Color(1f, 1f, 1f, 0.18f);
            var layout = go.GetComponent<LayoutElement>();
            layout.preferredWidth = 64f;
            layout.preferredHeight = 64f;
        }

        void AddRankRow(string rank, string name, string value, bool isMe)
        {
            if (rankContent == null) return;

            var row = new GameObject($"Rank_{rank}", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            row.transform.SetParent(rankContent, false);
            row.GetComponent<Image>().color = isMe ? new Color(0.12f, 0.45f, 0.9f, 0.75f) : new Color(1f, 1f, 1f, 0.08f);
            row.GetComponent<LayoutElement>().preferredHeight = 54f;
            var layout = row.GetComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 6, 6);
            layout.spacing = 12f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            AddLabel(row.transform, rank, 0.28f, TextAlignmentOptions.MidlineLeft);
            AddLabel(row.transform, name, 1f, TextAlignmentOptions.MidlineLeft);
            AddLabel(row.transform, value, 0.45f, TextAlignmentOptions.MidlineRight);
        }

        static void AddLabel(Transform parent, string text, float flexibleWidth, TextAlignmentOptions alignment)
        {
            var label = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            label.transform.SetParent(parent, false);
            var tmp = label.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 22f;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 14f;
            tmp.fontSizeMax = 22f;
            tmp.color = Color.white;
            tmp.alignment = alignment;
            label.GetComponent<LayoutElement>().flexibleWidth = flexibleWidth;
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

        RectTransform FindRect(string childName)
        {
            foreach (var rect in GetComponentsInChildren<RectTransform>(true))
                if (rect.name == childName) return rect;
            return null;
        }

        static void SetText(TextMeshProUGUI label, string value)
        {
            if (label != null)
                label.text = value ?? "";
        }

        static void ClearChildren(RectTransform parent)
        {
            if (parent == null) return;
            for (int i = parent.childCount - 1; i >= 0; i--)
                Destroy(parent.GetChild(i).gameObject);
        }

        static string FormatScore(long value)
            => value.ToString("N0", CultureInfo.InvariantCulture);
    }
}
