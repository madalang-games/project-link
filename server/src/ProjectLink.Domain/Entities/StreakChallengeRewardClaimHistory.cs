namespace ProjectLink.Domain.Entities;

public class StreakChallengeRewardClaimHistory
{
    public long           Id                  { get; set; }
    public string         UserId              { get; set; } = default!;
    public int            EventId             { get; set; }
    public string         CycleId             { get; set; } = default!;
    public int            LevelIndex          { get; set; }
    public int            RewardGroupId       { get; set; }
    public int            RewardGroupVersion  { get; set; }
    public int            RewardMultiplier    { get; set; } = 1;
    public string         ClaimStatus         { get; set; } = "PENDING";
    public string         CorrelationId       { get; set; } = default!;
    public DateTimeOffset CreatedAt           { get; set; }
    public DateTimeOffset UpdatedAt           { get; set; }
}
