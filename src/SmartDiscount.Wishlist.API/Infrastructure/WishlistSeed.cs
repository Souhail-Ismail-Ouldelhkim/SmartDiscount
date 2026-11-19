using SmartDiscount.Wishlist.API.Infrastructure;

namespace SmartDiscount.Wishlist.API.Infrastructure;

public class WishlistSeed : IDbSeeder<WishlistContext>
{
    public Task SeedAsync(WishlistContext context)
    {
        return Task.CompletedTask;
    }
}