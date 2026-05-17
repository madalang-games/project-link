namespace ProjectLink.Domain.Entities;

public class StreakChallengeAdRewardHistory
{
    public long           Id               { get; set; }
    public string         UserId           { get; set; } = default!;
    public int            EventId          { get; set; }
    public string         CycleId          { get; set; } = default!;
    public int            LevelIndex       { get; set; }
    public string         AdPlacementId    { get; set; } = default!;
    public string         AdResult         { get; set; } = default!;
    public bool           MultiplierApplied { get; set; }
    public DateTimeOffset CreatedAt        { get; set; }
}
