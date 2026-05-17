#nullable enable

using System.Collections.Generic;

namespace ProjectLink.Contracts.StreakChallenge
{

// ── enums (string constants) ─────────────────────────────────────────────────

// EventStatus
// INACTIVE    — no active cycle
// ACTIVE      — cycle running, within expiry
// COMPLETED   — all levels cleared and all rewards claimed
// EXPIRED     — cycle existed but time ran out (lazy reset has fired)

// LevelStatus
// LOCKED      — not yet reachable (previous level not completed)
// READY       — previous level completed; waiting for user to press Start
// STARTED     — user pressed Start; stage clears now count
// COMPLETED   — required count reached

// RewardState
// NONE        — level not yet completed
// PENDING     — level completed, reward not yet claimed
// CLAIMED     — reward claimed
// EXPIRED     — event expired before claim

// NavigationDirective
// NONE                    — no navigation change needed
// RETURN_TO_LOBBY         — client must navigate to lobby
// OPEN_EVENT_POPUP        — open main streak challenge popup
// OPEN_REWARD_POPUP       — open reward claim popup for given level
// OPEN_LEVEL_START_POPUP  — open level start confirmation popup
// OPEN_FAILURE_POPUP      — open failure explanation popup
// OPEN_EXPIRATION_POPUP   — open expiration / new cycle popup

// AvailableAction
// ACTIVATE         — user can start a new cycle
// START_LEVEL      — user can start the ready level
// CONTINUE_LEVEL   — current level is started, stages count
// CLAIM_REWARD     — a reward is pending and can be claimed
// WATCH_AD         — ad multiplier available for pending reward
// VIEW_COMPLETED   — event is complete (read-only view)
// START_NEW_CYCLE  — event expired, user can begin again

// ExclusionReason
// EVENT_INACTIVE      — event not active
// EVENT_EXPIRED       — event time ran out
// LEVEL_NOT_STARTED   — level exists but not started by user
// NOT_MAIN_STAGE      — stage type excluded
// ALREADY_CLEARED     — stage already cleared before this cycle started
// DUPLICATE_RESULT    — same result submitted twice
// SERVER_VALIDATION_FAILED
// METADATA_UNAVAILABLE
// INVALID_EVENT_STATE

// ─────────────────────────────────────────────────────────────────────────────

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
    public string ItemType { get; set; } = "";  // SOFT_CURRENCY | ITEM
    public int    ItemId   { get; set; }
    public int    Amount   { get; set; }
}

public class StreakChallengeStateResponse
{
    public int                              EventId             { get; set; }
    public string                           EventStatus         { get; set; } = "INACTIVE";
    public string                           RemainingTimeIso    { get; set; } = "";   // ISO 8601 duration or empty
    public string                           ExpiresAtIso        { get; set; } = "";   // ISO 8601 datetime or empty
    public string                           CycleId             { get; set; } = "";
    public int                              EventVersion        { get; set; }
    public int                              CurrentLevel        { get; set; }
    public List<StreakChallengeLevelState>  Levels              { get; set; } = new();
    public List<string>                     AvailableActions    { get; set; } = new();
    public string                           NavigationDirective { get; set; } = "NONE";
    public int                              NavigationLevel     { get; set; }          // which level the directive targets
    public string?                          ExclusionReason     { get; set; }
    public int                              AdMultiplier        { get; set; } = 2;     // multiplier value from metadata
    public bool                             AdUsedThisLevel     { get; set; }
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
    public List<StreakChallengeRewardItem> RewardsGranted      { get; set; } = new();
    public long                           SoftBalanceAfter    { get; set; }
    public List<StreakChallengeInventoryUpdate> InventoryUpdates { get; set; } = new();
    public StreakChallengeStateResponse   EventState          { get; set; } = new();
    public bool                           MultiplierApplied   { get; set; }
}

public class StreakChallengeInventoryUpdate
{
    public int ItemId        { get; set; }
    public int QuantityAfter { get; set; }
}

}
