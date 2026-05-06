namespace ProjectLink.Domain.Interfaces;

public interface ISessionRepository
{
    Task<string?> GetCurrentSessionIdAsync(string userId, CancellationToken ct);
    Task CreateSessionAsync(string userId, string sessionId, DateTimeOffset expiresAt, CancellationToken ct);
    Task InvalidateAsync(string userId, CancellationToken ct);
}
