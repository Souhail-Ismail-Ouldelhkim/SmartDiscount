using Microsoft.EntityFrameworkCore;
using SmartDiscount.Discount.API.Data;

namespace SmartDiscount.Discount.API.Apis;

public static class DiscountApi
{
    public static IEndpointRouteBuilder MapDiscountApi(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/discount/validate", ValidateCodeAsync);
        return app;
    }

    private static async Task<IResult> ValidateCodeAsync(
        ValidateRequest request, DiscountContext db)
    {
        var promo = await db.PromoCodes
            .FirstOrDefaultAsync(p => p.Code == request.Code && p.IsActive);

        if (promo == null)
        {
            return Results.Ok(new ValidateResponse(false, 0, request.OrderTotal, "Code invalide ou inactif"));
        }

        if (promo.ExpirationDate.HasValue && promo.ExpirationDate.Value < DateTime.UtcNow)
        {
            return Results.Ok(new ValidateResponse(false, 0, request.OrderTotal, "Code expiré"));
        }
        if (promo.UsageMax.HasValue && promo.UsageCount >= promo.UsageMax.Value)
        {
            return Results.Ok(new ValidateResponse(false, 0, request.OrderTotal, "Code épuisé"));
        }

        decimal discount = promo.Type == "Percentage"
            ? request.OrderTotal * promo.Value / 100
            : promo.Value;

        if (discount > request.OrderTotal) discount = request.OrderTotal;

        var newTotal = request.OrderTotal - discount;

        return Results.Ok(new ValidateResponse(true, discount, newTotal, "Code appliqué"));
    }
}
public record ValidateRequest(string Code, decimal OrderTotal);
public record ValidateResponse(bool Valid, decimal DiscountAmount, decimal NewTotal, string Message);