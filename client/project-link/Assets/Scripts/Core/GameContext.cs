namespace ProjectLink.Core
{
    public static class GameContext
    {
        public static int SelectedStageId { get; set; } = 1;
        public static string StageSessionToken { get; set; } = "";
        public static long StageStartedAtMs { get; set; }
        public static int MoveLimit { get; set; }
        public static int TimeLimitSeconds { get; set; }
        public static bool IsStreakChallengeActive      { get; set; }
        public static bool ShouldOpenStreakPopupOnLobby { get; set; }
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
