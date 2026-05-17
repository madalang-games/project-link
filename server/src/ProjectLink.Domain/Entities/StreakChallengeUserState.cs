namespace ProjectLink.Domain.Entities;

public class StreakChallengeUserState
{
    public string         UserId             { get; set; } = default!;
    public int            EventId            { get; set; }
    public string         CycleId            { get; set; } = default!;
    public int            EventVersion       { get; set; }
    public string         EventStatus        { get; set; } = "INACTIVE";
    public DateTimeOffset ActivatedAt        { get; set; }
    public DateTimeOffset ExpiresAt          { get; set; }
    public int            CurrentLevel       { get; set; }
    public int            LastCountedStageId { get; set; }
    public DateTimeOffset CreatedAt          { get; set; }
    public DateTimeOffset UpdatedAt          { get; set; }
}
