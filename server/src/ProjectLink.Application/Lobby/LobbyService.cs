using ProjectLink.Application.StreakChallenge;
using ProjectLink.Contracts.Lobby;
using ProjectLink.Domain.Interfaces;

namespace ProjectLink.Application.Lobby;

public class LobbyService
{
    private readonly IUserProfileRepository   _profiles;
    private readonly IStaminaRepository       _stamina;
    private readonly ICurrencyRepository      _currency;
    private readonly IProgressRepository      _progress;
    private readonly StreakChallengeService   _streakChallenge;
    private readonly IStaticDataService       _staticData;

    public LobbyService(
        IUserProfileRepository  profiles,
        IStaminaRepository      stamina,
        ICurrencyRepository     currency,
        IProgressRepository     progress,
        StreakChallengeService  streakChallenge,
        IStaticDataService      staticData)
    {
        _profiles        = profiles;
        _stamina         = stamina;
        _currency        = currency;
        _progress        = progress;
        _streakChallenge = streakChallenge;
        _staticData      = staticData;
    }

    public async Task<LobbyStateResponse> GetAsync(string userId, CancellationToken ct)
    {
        var config = _staticData.GetStaminaConfig();

        var profile       = await _profiles.GetByIdAsync(userId, ct);
        var stamina       = await _stamina.GetComputedAsync(userId, config.MaxStamina, config.RechargeSeconds / 60, ct);
        var balance       = await _currency.GetBalanceAsync(userId, ct);
        var cleared       = (await _progress.GetAllAsync(userId, ct)).ToList();
        var streakSnapshot = await _streakChallenge.GetLobbySnapshotAsync(userId, ct);

        var highestSequential = profile?.MaxClearedStageId ?? 0;
        var totalStars        = cleared.Sum(p => p.Stars);
        var allStages         = _staticData.GetAllStages();
        var maxStageId        = allStages.Count > 0 ? allStages.Max(s => s.StageId) : 0;
        var nextUnlocked      = Math.Min(highestSequential + 1, maxStageId);

        var nextRecharge = stamina.Current < config.MaxStamina
            ? stamina.LastRechargedAt.AddSeconds(config.RechargeSeconds)
            : (DateTimeOffset?)null;

        var activeEvent = _staticData.GetAllSeasonEvents()
            .FirstOrDefault(e =>
            {
                var now = DateTimeOffset.UtcNow;
                return DateTimeOffset.TryParse(e.StartAt, out var start) &&
                       DateTimeOffset.TryParse(e.EndAt,   out var end)   &&
                       now >= start && now < end;
            });

        return new LobbyStateResponse
        {
            Profile = new LobbyProfile
            {
                DisplayName = profile?.DisplayName ?? "",
                AvatarId    = profile?.AvatarId ?? 1,
            },
            Stamina = new LobbyStamina
            {
                Current        = stamina.Current,
                Max            = config.MaxStamina,
                NextRechargeAt = nextRecharge?.ToString("O"),
            },
            Currency = new LobbyCurrency
            {
                SoftAmount = balance,
            },
            ProgressSummary = new LobbyProgressSummary
            {
                HighestStageId      = highestSequential,
                TotalStarsEarned    = totalStars,
                NextUnlockedStageId = nextUnlocked,
            },
            StreakChallenge = new LobbyStreakChallenge
            {
                EventStatus          = streakSnapshot.EventStatus,
                RemainingTimeIso     = streakSnapshot.RemainingTimeIso,
                CurrentLevel         = streakSnapshot.CurrentLevel,
                CurrentLevelCount    = streakSnapshot.CurrentLevelCount,
                CurrentLevelRequired = streakSnapshot.CurrentLevelRequired,
                HasPendingReward     = streakSnapshot.HasPendingReward,
            },
            SeasonEvent = activeEvent is null ? null : new LobbySeasonEvent
            {
                EventId  = activeEvent.EventId,
                Name     = activeEvent.Name,
                EndAt    = activeEvent.EndAt,
                IsActive = true,
            },
        };
    }
}
