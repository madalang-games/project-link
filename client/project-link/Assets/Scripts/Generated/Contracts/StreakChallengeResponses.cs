#nullable enable

using System.Collections.Generic;

namespace ProjectLink.Contracts.StreakChallenge
{

public class StreakChallengeLevelState
{
    public int    LevelIndex    { get; set; }
    public string LevelStatus   { get; set; } = "LOCKED";
    public int    RequiredCount { get; set; }
    public int    CurrentCount  { get; set; }
    public string RewardState   { get; set; } = "NONE";
    public int    RewardGroupId { get; set; }
}

public class StreakChallengeRewardItem
{
    public string ItemType { get; set; } = "";
    public int    ItemId   { get; set; }
    public int    Amount   { get; set; }
}

public class StreakChallengeStateResponse
{
    public int                             EventId             { get; set; }
    public string                          EventStatus         { get; set; } = "INACTIVE";
    public string                          RemainingTimeIso    { get; set; } = "";
    public string                          ExpiresAtIso        { get; set; } = "";
    public string                          CycleId             { get; set; } = "";
    public int                             EventVersion        { get; set; }
    public int                             CurrentLevel        { get; set; }
    public List<StreakChallengeLevelState> Levels              { get; set; } = new List<StreakChallengeLevelState>();
    public List<string>                    AvailableActions    { get; set; } = new List<string>();
    public string                          NavigationDirective { get; set; } = "NONE";
    public int                             NavigationLevel     { get; set; }
    public string?                         ExclusionReason     { get; set; }
    public int                             AdMultiplier        { get; set; } = 2;
    public bool                            AdUsedThisLevel     { get; set; }
}

public class StreakChallengeStageResultResponse
{
    public bool                           Counted             { get; set; }
    public string?                        ExclusionReason     { get; set; }
    public StreakChallengeStateResponse?  EventState          { get; set; }
    public string                         NavigationDirective { get; set; } = "NONE";
    public int                            NavigationLevel     { get; set; }
}

public class StreakChallengeClaimRewardResponse
{
    public List<StreakChallengeRewardItem>       RewardsGranted   { get; set; } = new List<StreakChallengeRewardItem>();
    public long                                  SoftBalanceAfter { get; set; }
    public List<StreakChallengeInventoryUpdate>  InventoryUpdates { get; set; } = new List<StreakChallengeInventoryUpdate>();
    public StreakChallengeStateResponse          EventState       { get; set; } = new StreakChallengeStateResponse();
    public bool                                  MultiplierApplied { get; set; }
}

public class StreakChallengeInventoryUpdate
{
    public int ItemId        { get; set; }
    public int QuantityAfter { get; set; }
}

}
