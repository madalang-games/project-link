namespace ProjectLink.Domain.Interfaces;

public class ShopPurchaseDbResult
{
    public long SoftBalanceAfter { get; set; }
    public int  QuantityAfter    { get; set; }
}

public interface IShopPurchaseTransaction
{
    Task<ShopPurchaseDbResult> ExecuteAsync(
        string userId,
        int    itemId,
        int    quantity,
        long   totalCost,
        string correlationId,
        CancellationToken ct);
}
