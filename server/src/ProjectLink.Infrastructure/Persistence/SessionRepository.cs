using Microsoft.EntityFrameworkCore;
using ProjectLink.Domain.Entities;
using ProjectLink.Domain.Interfaces;

namespace ProjectLink.Infrastructure.Persistence;

public class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _db;

    public SessionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<string?> GetCurrentSessionIdAsync(string userId, CancellationToken ct)
        => await _db.Sessions
            .Where(s => s.UserId == userId && s.Active && s.ExpiresAt > DateTimeOffset.UtcNow)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => s.SessionId)
            .FirstOrDefaultAsync(ct);

    public async Task CreateSessionAsync(string userId, string sessionId, DateTimeOffset expiresAt, CancellationToken ct)
    {
        _db.Sessions.Add(new Session
        {
            UserId    = userId,
            SessionId = sessionId,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = expiresAt,
            Active    = true,
        });
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> TryCreateSessionAsync(string userId, string sessionId, DateTimeOffset expiresAt, CancellationToken ct)
    {
        var now  = DateTimeOffset.UtcNow;
        var rows = await _db.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT IGNORE INTO sessions (user_id, session_id, created_at, expires_at, active) VALUES ({userId}, {sessionId}, {now}, {expiresAt}, 1)",
            ct);
        return rows > 0;
    }

    public async Task InvalidateAsync(string userId, CancellationToken ct)
    {
        await _db.Sessions
            .Where(s => s.UserId == userId && s.Active)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Active, false), ct);
    }
}
