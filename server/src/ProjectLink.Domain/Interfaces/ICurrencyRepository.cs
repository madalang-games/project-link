namespace ProjectLink.Domain.Interfaces;

public interface ICurrencyRepository
{
    Task<long> GetBalanceAsync(string userId, CancellationToken ct);
    Task<long> GrantAsync(string userId, long amount, string reason, string transactionId, string correlationId, CancellationToken ct);
    Task<long> DeductAsync(string userId, long amount, string reason, string transactionId, string correlationId, CancellationToken ct);
}
