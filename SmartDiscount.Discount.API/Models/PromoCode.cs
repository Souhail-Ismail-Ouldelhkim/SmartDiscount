namespace SmartDiscount.Discount.API.Models;

public class PromoCode
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string Type { get; set; } = "Percentage";   
    public decimal Value { get; set; }                  
    public DateTime? ExpirationDate { get; set; }
    public int? UsageMax { get; set; }                  
    public int UsageCount { get; set; }
    public bool IsActive { get; set; } = true;
}