using System.Net.Http.Json;

namespace SmartDiscount.WebApp.Services;

public class DiscountService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "/api/discount/";

    public async Task<DiscountResult?> ValidateCodeAsync(string code, decimal orderTotal)
    {
        var request = new ValidateRequest(code, orderTotal);
        var response = await httpClient.PostAsJsonAsync($"{remoteServiceBaseUrl}validate", request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<DiscountResult>();
    }
}

public record ValidateRequest(string Code, decimal OrderTotal);

public record DiscountResult(
    bool Valid,
    double DiscountAmount,
    decimal NewTotal,
    string Message);