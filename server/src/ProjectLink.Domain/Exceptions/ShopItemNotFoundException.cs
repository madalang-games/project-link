namespace ProjectLink.Domain.Exceptions;

public class ShopItemNotFoundException : DomainException
{
    public ShopItemNotFoundException()
        : base("SHOP_ITEM_NOT_FOUND", "The requested shop product does not exist or is unavailable.") { }
}
