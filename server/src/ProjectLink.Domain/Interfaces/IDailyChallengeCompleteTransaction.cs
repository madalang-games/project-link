namespace ProjectLink.Domain.Interfaces;

public class DailyChallengeRewardInput
{
    public string RewardType { get; set; } = "";
    public int    RewardId   { get; set; }
    public int    Amount     { get; set; }
}

public class DailyChallengeCompleteDbResult
{
    public int                  NewStreakDays    { get; set; }
    public long                 SoftBalanceAfter { get; set; }
    public Dictionary<int, int> InventoryAfter   { get; set; } = new();
}

public interface IDailyChallengeCompleteTransaction
{
    Task<DailyChallengeCompleteDbResult> ExecuteAsync(
        string                      userId,
        DateOnly                    challengeDate,
        int                         currentStreak,
        List<DailyChallengeRewardInput> rewards,
        string                      correlationId,
        CancellationToken           ct);
}
