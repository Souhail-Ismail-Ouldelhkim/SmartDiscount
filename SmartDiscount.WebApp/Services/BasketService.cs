using SmartDiscount.Basket.API.Grpc;
using GrpcBasketItem = SmartDiscount.Basket.API.Grpc.BasketItem;
using GrpcBasketClient = SmartDiscount.Basket.API.Grpc.Basket.BasketClient;

namespace SmartDiscount.WebApp.Services;

public class BasketService(GrpcBasketClient basketClient)
{
    public async Task<BasketData> GetBasketAsync()
    {
        var result = await basketClient.GetBasketAsync(new());
        return MapToBasket(result);
    }

    public async Task DeleteBasketAsync()
    {
        await basketClient.DeleteBasketAsync(new DeleteBasketRequest());
    }

    public async Task UpdateBasketAsync(IReadOnlyCollection<BasketQuantity> basket, string? promoCode = null, double discountAmount = 0)
    {
        var updatePayload = new UpdateBasketRequest
        {
            PromoCode = promoCode ?? "",
            DiscountAmount = discountAmount
        };
        foreach (var item in basket)
        {
            var updateItem = new GrpcBasketItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
            };
            updatePayload.Items.Add(updateItem);
        }
        await basketClient.UpdateBasketAsync(updatePayload);
    }

    private static BasketData MapToBasket(CustomerBasketResponse response)
    {
        var items = new List<BasketQuantity>();
        foreach (var item in response.Items)
        {
            items.Add(new BasketQuantity(item.ProductId, item.Quantity));
        }
        return new BasketData(items, response.PromoCode, response.DiscountAmount);
    }
}

public record BasketQuantity(int ProductId, int Quantity);
public record BasketData(List<BasketQuantity> Items, string PromoCode, double DiscountAmount);