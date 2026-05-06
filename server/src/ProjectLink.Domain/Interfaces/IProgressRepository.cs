using ProjectLink.Domain.Entities;

namespace ProjectLink.Domain.Interfaces;

public interface IProgressRepository
{
    Task<IEnumerable<StageProgress>> GetAllAsync(string userId, CancellationToken ct);
    Task UpsertBatchAsync(string userId, IEnumerable<StageProgress> records, CancellationToken ct);
}
