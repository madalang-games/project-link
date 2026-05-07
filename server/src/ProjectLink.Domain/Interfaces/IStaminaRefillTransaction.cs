namespace ProjectLink.Domain.Interfaces;

public class StaminaRefillDbResult
{
    public int            CurrentAfter    { get; set; }
    public int            Added           { get; set; }
    public long           SoftBalanceAfter { get; set; }
    public DateTimeOffset? NextRechargeAt { get; set; }
}

public interface IStaminaRefillTransaction
{
    Task<StaminaRefillDbResult> ExecuteAsync(
        string userId,
        int    maxStamina,
        int    rechargeSeconds,
        int    refillCostSoft,
        string correlationId,
        CancellationToken ct);
}
