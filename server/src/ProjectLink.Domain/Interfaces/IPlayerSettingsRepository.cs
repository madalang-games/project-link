using ProjectLink.Domain.Entities;

namespace ProjectLink.Domain.Interfaces;

public interface IPlayerSettingsRepository
{
    Task<PlayerSettings> GetOrDefaultAsync(string userId, CancellationToken ct);
    Task UpsertAsync(PlayerSettings settings, CancellationToken ct);
}
