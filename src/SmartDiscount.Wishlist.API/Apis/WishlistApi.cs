using Microsoft.EntityFrameworkCore;
using SmartDiscount.Wishlist.API.Infrastructure;
using SmartDiscount.Wishlist.API.Model;

namespace SmartDiscount.Wishlist.API.Apis;

public static class WishlistApi
{
    public static void MapWishlistApi(this WebApplication app)
    {
        app.MapPost("/api/wishlist", AddToWishlistAsync);

        app.MapGet("/api/wishlist/{userId}", GetWishlistAsync);

        app.MapPut("/api/wishlist/{id:int}/remove", RemoveAsync);

        app.MapPut("/api/wishlist/{id:int}/move-to-cart", MoveToCartAsync);
    }

    private static async Task<IResult> AddToWishlistAsync(AddWishlistRequest request, WishlistContext db)
    {
        
        var existing = await db.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == request.UserId
                                   && w.ProductId == request.ProductId
                                   && w.Status == WishlistStatus.Active);
        if (existing is not null)
        {
            return Results.Ok(new { message = "Déjà dans les favoris" });
        }

        var item = new WishlistItem
        {
            UserId = request.UserId,
            ProductId = request.ProductId,
            DateAdded = DateTime.UtcNow,
            Status = WishlistStatus.Active
        };
        db.WishlistItems.Add(item);
        await db.SaveChangesAsync();
        return Results.Ok(new { item.Id });
    }

    private static async Task<IResult> GetWishlistAsync(string userId, WishlistContext db)
    {
        var items = await db.WishlistItems
            .Where(w => w.UserId == userId && w.Status == WishlistStatus.Active)
            .OrderByDescending(w => w.DateAdded)
            .Select(w => new WishlistItemDto(w.Id, w.ProductId, w.DateAdded))
            .ToListAsync();
        return Results.Ok(items);
    }

    private static async Task<IResult> RemoveAsync(int id, WishlistContext db)
    {
        var item = await db.WishlistItems.FindAsync(id);
        if (item is null) return Results.NotFound();

        item.Status = WishlistStatus.Removed;
        item.DateStatusChanged = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Results.Ok();
    }

    private static async Task<IResult> MoveToCartAsync(int id, WishlistContext db)
    {
        var item = await db.WishlistItems.FindAsync(id);
        if (item is null) return Results.NotFound();

        item.Status = WishlistStatus.AddedToCart;
        item.DateStatusChanged = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Results.Ok();
    }
}

public record AddWishlistRequest(string UserId, int ProductId);
public record WishlistItemDto(int Id, int ProductId, DateTime DateAdded);