#nullable enable

namespace ProjectLink.Contracts.Lobby
{

public class LobbyStateResponse
{
    public LobbyProfile         Profile         { get; set; } = new();
    public LobbyStamina         Stamina         { get; set; } = new();
    public LobbyCurrency        Currency        { get; set; } = new();
    public LobbyProgressSummary ProgressSummary { get; set; } = new();
    public LobbyStreakChallenge  StreakChallenge { get; set; } = new();
    public LobbySeasonEvent?    SeasonEvent     { get; set; }
}

public class LobbyProfile
{
    public string DisplayName { get; set; } = "";
    public int    AvatarId    { get; set; }
}

public class LobbyStamina
{
    public int     Current        { get; set; }
    public int     Max            { get; set; }
    public string? NextRechargeAt { get; set; }
}

public class LobbyCurrency
{
    public long SoftAmount { get; set; }
}

public class LobbyProgressSummary
{
    public int HighestStageId      { get; set; }
    public int TotalStarsEarned    { get; set; }
    public int NextUnlockedStageId { get; set; }
}

public class LobbyStreakChallenge
{
    public string EventStatus         { get; set; } = "INACTIVE";  // INACTIVE/ACTIVE/COMPLETED/EXPIRED
    public string RemainingTimeIso    { get; set; } = "";           // ISO 8601 duration; empty if inactive
    public int    CurrentLevel        { get; set; }
    public int    CurrentLevelCount   { get; set; }
    public int    CurrentLevelRequired { get; set; }
    public bool   HasPendingReward    { get; set; }
}

public class LobbySeasonEvent
{
    public int    EventId  { get; set; }
    public string Name     { get; set; } = "";
    public string EndAt    { get; set; } = "";
    public bool   IsActive { get; set; }
}
}
