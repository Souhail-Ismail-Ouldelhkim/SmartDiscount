using Microsoft.EntityFrameworkCore;
using SmartDiscount.Wishlist.API.Infrastructure.EntityConfigurations;
using SmartDiscount.Wishlist.API.Model;

namespace SmartDiscount.Wishlist.API.Infrastructure;

public class WishlistContext(DbContextOptions<WishlistContext> options) : DbContext(options)
{
    public DbSet<WishlistItem> WishlistItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new WishlistItemConfiguration());
    }
}