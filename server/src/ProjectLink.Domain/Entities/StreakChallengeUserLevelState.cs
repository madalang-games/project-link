namespace ProjectLink.Domain.Entities;

public class StreakChallengeUserLevelState
{
    public string          UserId          { get; set; } = default!;
    public int             EventId         { get; set; }
    public string          CycleId         { get; set; } = default!;
    public int             LevelIndex      { get; set; }
    public string          LevelStatus     { get; set; } = "LOCKED";
    public int             RequiredCount   { get; set; }
    public int             CurrentCount    { get; set; }
    public DateTimeOffset? StartedAt       { get; set; }
    public DateTimeOffset? CompletedAt     { get; set; }
    public string          RewardState     { get; set; } = "NONE";
    public DateTimeOffset? RewardClaimedAt { get; set; }
    public DateTimeOffset  UpdatedAt       { get; set; }
}
