namespace ProjectLink.Domain.StaticData;

public class StreakChallengeRewardGroupData
{
    public int    RewardGroupId      { get; set; }
    public int    RewardGroupVersion { get; set; }
    public string RewardType        { get; set; } = "";
    public bool   IsEnabled          { get; set; }
}
