using Microsoft.EntityFrameworkCore;
using SmartDiscount.Discount.API.Models;

namespace SmartDiscount.Discount.API.Data;

public class DiscountContext : DbContext
{
    public DiscountContext(DbContextOptions<DiscountContext> options) : base(options)
    {
    }

    public DbSet<PromoCode> PromoCodes { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<PromoCode>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(p => p.Code).IsUnique();   
            entity.Property(p => p.Type).HasMaxLength(20);
            entity.Property(p => p.Value).HasColumnType("decimal(18,2)");
        });
    }
}