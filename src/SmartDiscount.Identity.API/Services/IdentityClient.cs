using System.Text.Json;

namespace SmartDiscount.Notification.API.Services;

public class UserInfo
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string Name { get; set; } = "";
    public string? LastName { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
}

public class IdentityClient(HttpClient httpClient, ILogger<IdentityClient> logger)
{
    public async Task<UserInfo?> GetUserByIdAsync(string guid)
    {
        try
        {
            var response = await httpClient.GetAsync($"/api/users/{guid}");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Impossible de récupérer l'user {Guid} : {Status}", guid, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserInfo>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de l'appel à Identity pour l'user {Guid}", guid);
            return null;
        }
    }
}