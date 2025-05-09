
namespace Basket.API.Data
{
    public class BasketRepository(IDocumentSession session)
        : IBasketRepository
    {
        public async Task<ShoppingCart> GetBasket(string userName, CancellationToken ct)
        {
            var basket = await session.LoadAsync<ShoppingCart>(userName, ct);

            return basket is null ? throw new BasketNotFoundException(userName) : basket;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken ct)
        {
            session.Store(basket);
            await session.SaveChangesAsync(ct);
            return basket;
        }
        public async Task<bool> DeleteBasket(string userName, CancellationToken ct)
        {
            session.Delete<ShoppingCart>(userName);
            await session.SaveChangesAsync(ct);
            return true;
        }
    }
}
