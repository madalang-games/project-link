using ProjectLink.Domain.Entities;
using ProjectLink.Domain.StaticData;

namespace ProjectLink.Domain.Interfaces;

public class StreakChallengeActivateCommand
{
    public string         UserId       { get; set; } = default!;
    public int            EventId      { get; set; }
    public string         CycleId      { get; set; } = default!;
    public int            EventVersion { get; set; }
    public DateTimeOffset ActivatedAt  { get; set; }
    public DateTimeOffset ExpiresAt    { get; set; }
    public List<StreakChallengeLevelData> Levels { get; set; } = new();
}

public class StreakChallengeStartLevelCommand
{
    public string UserId     { get; set; } = default!;
    public int    EventId    { get; set; }
    public string CycleId    { get; set; } = default!;
    public int    LevelIndex { get; set; }
}

public class StreakChallengeProgressCommand
{
    public string UserId       { get; set; } = default!;
    public int    EventId      { get; set; }
    public string CycleId      { get; set; } = default!;
    public int    LevelIndex   { get; set; }
    public int    StageId      { get; set; }
    public bool   IsSuccess    { get; set; }
}

public class StreakChallengeProgressResult
{
    public bool   LevelCompleted    { get; set; }
    public bool   EventCompleted    { get; set; }
    public int    NewCount          { get; set; }
}

public class StreakChallengeClaimCommand
{
    public string UserId             { get; set; } = default!;
    public int    EventId            { get; set; }
    public string CycleId            { get; set; } = default!;
    public int    LevelIndex         { get; set; }
    public int    RewardGroupId      { get; set; }
    public int    RewardGroupVersion { get; set; }
    public int    RewardMultiplier   { get; set; } = 1;
    public string CorrelationId      { get; set; } = default!;
    public List<StreakChallengeRewardItemData> RewardItems { get; set; } = new();
}

public class StreakChallengeClaimResult
{
    public bool                 IsNewClaim       { get; set; }
    public long                 SoftBalanceAfter { get; set; }
    public Dictionary<int, int> InventoryAfter   { get; set; } = new();
    public bool                 EventCompleted   { get; set; }
}

public class StreakChallengeLazyResetCommand
{
    public string         UserId   { get; set; } = default!;
    public int            EventId  { get; set; }
    public string         CycleId  { get; set; } = default!;
    public DateTimeOffset ExpiredAt { get; set; }
}

public interface IStreakChallengeTransaction
{
    Task ActivateAsync(StreakChallengeActivateCommand cmd, CancellationToken ct);
    Task StartLevelAsync(StreakChallengeStartLevelCommand cmd, CancellationToken ct);
    Task<StreakChallengeProgressResult> RecordProgressAsync(StreakChallengeProgressCommand cmd, CancellationToken ct);
    Task<StreakChallengeClaimResult> ClaimRewardAsync(StreakChallengeClaimCommand cmd, CancellationToken ct);
    Task LazyResetAsync(StreakChallengeLazyResetCommand cmd, CancellationToken ct);
    Task RecordAdAsync(StreakChallengeAdRewardHistory entry, CancellationToken ct);
}
