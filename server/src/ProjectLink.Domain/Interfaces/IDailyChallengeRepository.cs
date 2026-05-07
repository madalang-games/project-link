using ProjectLink.Domain.Entities;

namespace ProjectLink.Domain.Interfaces;

public interface IDailyChallengeRepository
{
    Task<DailyChallengeProgress?> GetForDateAsync(string userId, DateOnly date, CancellationToken ct);
    Task<int>                     IncrementPlayCountAsync(string userId, DateOnly date, CancellationToken ct);
}
