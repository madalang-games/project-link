using ProjectLink.Domain.Entities;

namespace ProjectLink.Domain.Interfaces;

public interface IStreakChallengeRepository
{
    Task<StreakChallengeUserState?> GetActiveStateAsync(string userId, int eventId, CancellationToken ct);
    Task<List<StreakChallengeUserLevelState>> GetLevelStatesAsync(string userId, int eventId, string cycleId, CancellationToken ct);
    Task<bool> HasAdUsedForLevelAsync(string userId, int eventId, string cycleId, int levelIndex, CancellationToken ct);
    Task<StreakChallengeRewardClaimHistory?> GetClaimHistoryByCorrelationAsync(string correlationId, CancellationToken ct);
}
