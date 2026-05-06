using ProjectLink.Domain.Interfaces;

namespace ProjectLink.Application.Session;

public class SessionService
{
    private readonly ISessionRepository _sessionRepo;
    private readonly ISessionCache      _sessionCache;

    public SessionService(ISessionRepository sessionRepo, ISessionCache sessionCache)
    {
        _sessionRepo  = sessionRepo;
        _sessionCache = sessionCache;
    }

    public async Task<string> CreateSessionAsync(string userId, TimeSpan tokenLifetime, CancellationToken ct = default)
    {
        var sessionId = Guid.NewGuid().ToString();
        var expiresAt = DateTimeOffset.UtcNow.Add(tokenLifetime);

        await _sessionRepo.InvalidateAsync(userId, ct);
        await _sessionCache.DeleteAsync(userId);

        await _sessionRepo.CreateSessionAsync(userId, sessionId, expiresAt, ct);
        await _sessionCache.SetSessionIdAsync(userId, sessionId, tokenLifetime);

        return sessionId;
    }

    public async Task<bool> ValidateSessionAsync(string userId, string claimedSessionId, CancellationToken ct = default)
    {
        var cached = await _sessionCache.GetSessionIdAsync(userId);
        if (cached is not null)
            return cached == claimedSessionId;

        var stored = await _sessionRepo.GetCurrentSessionIdAsync(userId, ct);
        return stored == claimedSessionId;
    }
}
