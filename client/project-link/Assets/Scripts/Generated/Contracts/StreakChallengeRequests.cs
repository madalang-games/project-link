#nullable enable

namespace ProjectLink.Contracts.StreakChallenge
{

public class StreakChallengeActivateRequest { }

public class StreakChallengeStartLevelRequest { }

public class StreakChallengeClaimRewardRequest
{
    public string CorrelationId { get; set; } = "";
}

public class StreakChallengeClaimRewardWithAdRequest
{
    public string AdToken       { get; set; } = "";
    public string AdPlacementId { get; set; } = "";
    public string CorrelationId { get; set; } = "";
}

public class StreakChallengeRefreshRequest { }

}
