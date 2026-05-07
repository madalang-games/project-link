namespace ProjectLink.Domain.Entities;

public class DailyChallengeProgress
{
    public string         UserId         { get; set; } = default!;
    public DateOnly       ChallengeDate  { get; set; }
    public int            PlayCount      { get; set; }
    public bool           Completed      { get; set; }
    public int            StreakDays     { get; set; }
    public DateOnly?      LastStreakDate { get; set; }
    public DateTimeOffset CreatedAt      { get; set; }
}
