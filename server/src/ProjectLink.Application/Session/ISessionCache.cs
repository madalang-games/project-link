namespace ProjectLink.Application.Session;

public interface ISessionCache
{
    Task<string?> GetSessionIdAsync(string userId);
    Task SetSessionIdAsync(string userId, string sessionId, TimeSpan ttl);
    Task DeleteAsync(string userId);
}
