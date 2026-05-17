namespace ProjectLink.Domain.StaticData;

public class StreakChallengeEventData
{
    public int    EventId            { get; set; }
    public int    Version            { get; set; }
    public bool   IsEnabled          { get; set; }
    public int    DurationSeconds    { get; set; }
    public string ResetType         { get; set; } = "";
    public string StageCountPolicy  { get; set; } = "";
    public string RewardPolicy      { get; set; } = "";
    public string AdPolicy          { get; set; } = "";
}
