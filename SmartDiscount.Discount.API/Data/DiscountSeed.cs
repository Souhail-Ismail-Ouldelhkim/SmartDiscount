using Microsoft.EntityFrameworkCore;   // ← AJOUTER cette ligne
using SmartDiscount.Discount.API.Models;

namespace SmartDiscount.Discount.API.Data;

public class DiscountSeed : IDbSeeder<DiscountContext>
{
    public async Task SeedAsync(DiscountContext context)
    {
        if (!context.PromoCodes.Any())
        {
            var codes = new List<PromoCode>
            {
                new() { Code = "SUMMER25", Type = "Percentage", Value = 25, IsActive = true },
                new() { Code = "WELCOME10", Type = "Percentage", Value = 10, IsActive = true },
                new() { Code = "SAVE50", Type = "Percentage", Value = 50, IsActive = true },
                new() { Code = "EXPIRED", Type = "Percentage", Value = 30, IsActive = false }
            };

            await context.PromoCodes.AddRangeAsync(codes);
            await context.SaveChangesAsync();
        }
    }
}