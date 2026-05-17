using Microsoft.EntityFrameworkCore;
using ProjectLink.Domain.Entities;
using ProjectLink.Domain.Interfaces;

namespace ProjectLink.Infrastructure.Persistence;

public class StreakChallengeRepository : IStreakChallengeRepository
{
    private readonly AppDbContext _db;

    public StreakChallengeRepository(AppDbContext db) => _db = db;

    public async Task<StreakChallengeUserState?> GetActiveStateAsync(string userId, int eventId, CancellationToken ct)
        => await _db.StreakChallengeUserStates
            .Where(s => s.UserId == userId && s.EventId == eventId)
            .OrderByDescending(s => s.ActivatedAt)
            .FirstOrDefaultAsync(ct);

    public async Task<List<StreakChallengeUserLevelState>> GetLevelStatesAsync(
        string userId, int eventId, string cycleId, CancellationToken ct)
        => await _db.StreakChallengeUserLevelStates
            .Where(l => l.UserId == userId && l.EventId == eventId && l.CycleId == cycleId)
            .OrderBy(l => l.LevelIndex)
            .ToListAsync(ct);

    public async Task<bool> HasAdUsedForLevelAsync(
        string userId, int eventId, string cycleId, int levelIndex, CancellationToken ct)
        => await _db.StreakChallengeAdRewardHistories
            .AnyAsync(a => a.UserId == userId && a.EventId == eventId
                        && a.CycleId == cycleId && a.LevelIndex == levelIndex
                        && a.MultiplierApplied, ct);

    public async Task<StreakChallengeRewardClaimHistory?> GetClaimHistoryByCorrelationAsync(
        string correlationId, CancellationToken ct)
        => await _db.StreakChallengeRewardClaimHistories
            .FirstOrDefaultAsync(c => c.CorrelationId == correlationId, ct);
}
