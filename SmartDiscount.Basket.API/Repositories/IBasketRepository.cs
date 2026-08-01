using SmartDiscount.Basket.API.Model;
using System.Threading.Tasks;

namespace SmartDiscount.Basket.API.Repositories;

public interface IBasketRepository
{
    Task<CustomerBasket> GetBasketAsync(string customerId);
    Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
    Task<bool> DeleteBasketAsync(string id);
}
