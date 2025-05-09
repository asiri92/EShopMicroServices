namespace Basket.API.Data
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetBasket(string userName, CancellationToken ct);
        Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken ct);

        Task<bool> DeleteBasket(string userName, CancellationToken ct);
    }
}
