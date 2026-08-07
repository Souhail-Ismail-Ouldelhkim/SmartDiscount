using System.Text.Json;

namespace SmartDiscount.Notification.API.Services;

public class UserInfo
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string Name { get; set; } = "";
    public string? LastName { get; set; }
}

public class IdentityClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IdentityClient> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IdentityClient(HttpClient httpClient, ILogger<IdentityClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserInfo?> GetUserByIdAsync(string guid)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/users/{guid}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Impossible de recuperer l'user {Guid} : {Status}", guid, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserInfo>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'appel a Identity pour l'user {Guid}", guid);
            return null;
        }
    }
}