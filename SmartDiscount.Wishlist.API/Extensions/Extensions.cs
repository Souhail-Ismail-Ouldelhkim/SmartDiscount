using SmartDiscount.Wishlist.API.Infrastructure;

namespace SmartDiscount.Wishlist.API.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<WishlistContext>("wishlistdb");
    }
}