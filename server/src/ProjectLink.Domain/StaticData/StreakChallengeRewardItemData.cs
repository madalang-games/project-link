namespace ProjectLink.Domain.StaticData;

public class StreakChallengeRewardItemData
{
    public int    RewardGroupId      { get; set; }
    public int    RewardGroupVersion { get; set; }
    public string ItemType          { get; set; } = "";
    public int    ItemId            { get; set; }
    public int    Amount            { get; set; }
    public int    Weight            { get; set; }
    public int    DisplayOrder      { get; set; }
}
