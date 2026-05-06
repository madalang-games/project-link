using ProjectLink.Application.Session;
using StackExchange.Redis;

namespace ProjectLink.Infrastructure.Cache;

public class RedisSessionCache : ISessionCache
{
    private readonly IDatabase _db;

    public RedisSessionCache(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    private static string Key(string userId) => $"session:{userId}";

    public async Task<string?> GetSessionIdAsync(string userId)
    {
        var value = await _db.StringGetAsync(Key(userId));
        return value.HasValue ? (string?)value : null;
    }

    public Task SetSessionIdAsync(string userId, string sessionId, TimeSpan ttl)
        => _db.StringSetAsync(Key(userId), sessionId, ttl);

    public Task DeleteAsync(string userId)
        => _db.KeyDeleteAsync(Key(userId));
}
