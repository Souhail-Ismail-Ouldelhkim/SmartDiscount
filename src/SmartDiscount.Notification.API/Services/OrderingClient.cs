using System.Text.Json;

namespace SmartDiscount.Notification.API.Services;

public class OrderInfo
{
    public int OrderNumber { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string Zipcode { get; set; }
    public double Total { get; set; }
    public double DiscountAmount { get; set; }   
    public string PromoCode { get; set; }   
    public List<OrderItemInfo> OrderItems { get; set; } = new();
}

public class OrderItemInfo
{
    public string ProductName { get; set; }
    public int Units { get; set; }
    public double UnitPrice { get; set; }
}

public class OrderingClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderingClient> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OrderingClient(HttpClient httpClient, ILogger<OrderingClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OrderInfo> GetOrderByIdAsync(int orderId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/internal/orders/{orderId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Impossible de recuperer la commande {OrderId} : {Status}", orderId, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderInfo>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'appel a Ordering pour la commande {OrderId}", orderId);
            return null;
        }
    }
}