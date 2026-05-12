using System.Collections.Generic;
using UnityEngine;

namespace ProjectLink.Core
{
    public static class GameContext
    {
        public static int SelectedStageId { get; set; } = 1;
        public static string StageSessionToken { get; set; } = "";
        public static long StageStartedAtMs { get; set; }
        public static int MoveLimit { get; set; }
        public static int TimeLimitSeconds { get; set; }
        public static bool IsDailyChallengeStage { get; private set; }
        public static int DailyChallengeIndex { get; private set; }
        static readonly List<int> DailyChallengeStageIds = new();
        public static bool SuppressTitleSilentLoginOnce { get; private set; }

        public static void SetStageSession(string sessionToken, int moveLimit, int timeLimitSeconds)
        {
            StageSessionToken = sessionToken ?? "";
            MoveLimit = moveLimit;
            TimeLimitSeconds = timeLimitSeconds;
            StageStartedAtMs = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public static void ClearStageSession()
        {
            StageSessionToken = "";
            StageStartedAtMs = 0;
            MoveLimit = 0;
            TimeLimitSeconds = 0;
        }

        public static void SetDailyChallengeRun(IReadOnlyList<int> stageIds, int startIndex)
        {
            DailyChallengeStageIds.Clear();
            if (stageIds != null)
            {
                for (int i = 0; i < stageIds.Count; i++)
                {
                    if (stageIds[i] > 0)
                        DailyChallengeStageIds.Add(stageIds[i]);
                }
            }

            DailyChallengeIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, DailyChallengeStageIds.Count - 1));
            IsDailyChallengeStage = DailyChallengeStageIds.Count > 0;
        }

        public static bool TryGetNextDailyChallengeStage(out int nextStageId)
        {
            nextStageId = 0;
            if (!IsDailyChallengeStage)
                return false;

            int nextIndex = DailyChallengeIndex + 1;
            if (nextIndex < 0 || nextIndex >= DailyChallengeStageIds.Count)
                return false;

            nextStageId = DailyChallengeStageIds[nextIndex];
            return nextStageId > 0;
        }

        public static void AdvanceDailyChallengeStage(int stageId)
        {
            for (int i = 0; i < DailyChallengeStageIds.Count; i++)
            {
                if (DailyChallengeStageIds[i] != stageId) continue;
                DailyChallengeIndex = i;
                IsDailyChallengeStage = true;
                return;
            }
        }

        public static void ClearDailyChallengeRun()
        {
            DailyChallengeStageIds.Clear();
            DailyChallengeIndex = 0;
            IsDailyChallengeStage = false;
        }

        public static void SuppressNextTitleSilentLogin()
        {
            SuppressTitleSilentLoginOnce = true;
        }

        public static bool ConsumeTitleSilentLoginSuppression()
        {
            if (!SuppressTitleSilentLoginOnce)
                return false;

            SuppressTitleSilentLoginOnce = false;
            return true;
        }
    }
}
