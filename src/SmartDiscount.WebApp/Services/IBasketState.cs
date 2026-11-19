using SmartDiscount.WebAppComponents.Catalog;

namespace SmartDiscount.WebApp.Services
{
    public interface IBasketState
    {
        public Task<IReadOnlyCollection<BasketItem>> GetBasketItemsAsync();
        public Task AddAsync(CatalogItem item);
    }
}