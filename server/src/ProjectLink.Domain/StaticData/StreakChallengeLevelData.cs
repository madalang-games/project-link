namespace ProjectLink.Domain.StaticData;

public class StreakChallengeLevelData
{
    public int    EventId             { get; set; }
    public int    Version             { get; set; }
    public int    LevelIndex          { get; set; }
    public int    RequiredClearCount  { get; set; }
    public int    RewardGroupId       { get; set; }
    public bool   AllowTimeExtension  { get; set; }
    public bool   AllowRevive         { get; set; }
    public bool   IsEnabled           { get; set; }
    public int    DisplayOrder        { get; set; }
    public string LocalizationKey     { get; set; } = "";
}
