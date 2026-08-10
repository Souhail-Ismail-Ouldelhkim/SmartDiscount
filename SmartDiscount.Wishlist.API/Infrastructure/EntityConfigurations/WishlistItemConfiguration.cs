using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDiscount.Wishlist.API.Model;

namespace SmartDiscount.Wishlist.API.Infrastructure.EntityConfigurations;

public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        builder.ToTable("WishlistItems");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.UserId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.ProductId)
            .IsRequired();

        builder.Property(w => w.DateAdded)
            .IsRequired();

        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(w => new { w.UserId, w.Status });
    }
}