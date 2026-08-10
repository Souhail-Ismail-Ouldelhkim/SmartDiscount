using System.Net.Http.Json;

namespace SmartDiscount.WebApp.Services;

public class WishlistService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "/api/wishlist";

    public async Task AddToWishlistAsync(string userId, int productId)
    {
        var request = new AddWishlistRequest(userId, productId);
        await httpClient.PostAsJsonAsync(remoteServiceBaseUrl, request);
    }

    public async Task<List<WishlistItemDto>> GetWishlistAsync(string userId)
    {
        var result = await httpClient.GetFromJsonAsync<List<WishlistItemDto>>($"{remoteServiceBaseUrl}/{userId}");
        return result ?? new List<WishlistItemDto>();
    }

    public async Task RemoveAsync(int id)
    {
        await httpClient.PutAsync($"{remoteServiceBaseUrl}/{id}/remove", null);
    }

    public async Task MoveToCartAsync(int id)
    {
        await httpClient.PutAsync($"{remoteServiceBaseUrl}/{id}/move-to-cart", null);
    }
}

public record AddWishlistRequest(string UserId, int ProductId);
public record WishlistItemDto(int Id, int ProductId, DateTime DateAdded);