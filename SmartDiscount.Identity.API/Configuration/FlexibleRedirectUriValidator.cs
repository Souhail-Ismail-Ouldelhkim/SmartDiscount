using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartDiscount.Identity.API.Configuration;

public class FlexibleRedirectUriValidator : StrictRedirectUriValidator
{
    public override async Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
    {
        if (await base.IsRedirectUriValidAsync(requestedUri, client))
        {
            return true;
        }
        if (client.ClientId == "webapp")
        {
            var isAzureRevision = Regex.IsMatch(requestedUri,
                @"^https://webapp--\d+\.calmbay-[a-z0-9]+\.[a-z0-9]+\.azurecontainerapps\.io/signin-oidc$",
                RegexOptions.IgnoreCase);

            if (isAzureRevision)
            {
                return true;
            }
        }

        return false;
    }

    public override async Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
    {
        if (await base.IsPostLogoutRedirectUriValidAsync(requestedUri, client))
        {
            return true;
        }

        if (client.ClientId == "webapp")
        {
            var isAzureRevisionLogout = Regex.IsMatch(requestedUri,
                @"^https://webapp--\d+\.calmbay-[a-z0-9]+\.[a-z0-9]+\.azurecontainerapps\.io/signout-callback-oidc$",
                RegexOptions.IgnoreCase);

            if (isAzureRevisionLogout)
            {
                return true;
            }
        }

        return false;
    }
}