using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartDiscount.Identity.API.Models;
using System.Security.Claims;
using SmartDiscount.Identity.API.Models.AccountViewModels;

namespace IdentityServerHost.Quickstart.UI;

[SecurityHeaders]
[AllowAnonymous]
public class ExternalController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IClientStore _clientStore;
    private readonly IEventService _events;
    private readonly ILogger<ExternalController> _logger;

    public ExternalController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interaction,
        IClientStore clientStore,
        IEventService events,
        ILogger<ExternalController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _interaction = interaction;
        _clientStore = clientStore;
        _events = events;
        _logger = logger;
    }
    [HttpGet]
    public IActionResult Challenge(string scheme, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";
        if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
        {
            throw new Exception("invalid return URL");
        }

        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(Callback)),
            Items =
                {
                    { "returnUrl", returnUrl },
                    { "scheme", scheme },
                }
        };

        return Challenge(props, scheme);

    }

    [HttpGet]
    public async Task<IActionResult> Callback()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (result?.Succeeded != true)
        {
            throw new Exception("External authentication error");
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
            _logger.LogDebug("External claims: {@claims}", externalClaims);
        }

        var (user, provider, providerUserId, claims) = await FindUserFromExternalProviderAsync(result);

        if (user == null)
        {
            var returnUrlNew = result.Properties.Items["returnUrl"] ?? "~/";

            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value
                     ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var firstName = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value
                         ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            var lastName = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value
                        ?? claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

           
            HttpContext.Session.SetString("ext_provider", provider);
            HttpContext.Session.SetString("ext_providerUserId", providerUserId);
            HttpContext.Session.SetString("ext_email", email ?? "");
            HttpContext.Session.SetString("ext_returnUrl", returnUrlNew);

        
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            return RedirectToAction("EmailLogin", "External", new
            {
                email = email,
                firstName = firstName,
                lastName = lastName,
                returnUrl = returnUrlNew
            });
        }

        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);
        var principal = await _signInManager.CreateUserPrincipalAsync(user);
        additionalLocalClaims.AddRange(principal.Claims);
        var name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? user.Id;

        var isuser = new IdentityServerUser(user.Id)
        {
            DisplayName = name,
            IdentityProvider = provider,
            AdditionalClaims = additionalLocalClaims
        };

        await HttpContext.SignInAsync(isuser, localSignInProps);

        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, name, true, context?.Client.ClientId));

        if (context != null)
        {
            if (context.IsNativeClient())
            {
                return this.LoadingPage("Redirect", returnUrl);
            }
        }

        return Redirect(returnUrl);
    }

    [HttpGet]
    public IActionResult EmailLogin(string email, string firstName, string lastName, string returnUrl)
    {
        var vm = new EmailLoginViewModel
        {
            Email = email,
            Name = firstName,
            LastName = lastName,
            ReturnUrl = returnUrl
        };
        return View("~/Views/Account/EmailLogin.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EmailLogin(EmailLoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Account/EmailLogin.cshtml", model);

        var provider = HttpContext.Session.GetString("ext_provider");
        var providerUserId = HttpContext.Session.GetString("ext_providerUserId");

        if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(providerUserId))
        {
            ModelState.AddModelError(string.Empty, "External login session expired. Please try again.");
            return View("~/Views/Account/EmailLogin.cshtml", model);
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = model.UserName,
            Email = model.Email,
            EmailConfirmed = true,
            PhoneNumber = model.PhoneNumber,
            Name = model.Name,
            LastName = model.LastName,
            Street = model.Street,
            City = model.City,
            State = model.State,
            ZipCode = model.ZipCode,
            Country = model.Country,
            CardHolderName = model.CardHolderName,
            CardNumber = model.CardNumber,
            Expiration = model.Expiration,
            SecurityNumber = model.SecurityNumber,
            CardType = model.CardType
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View("~/Views/Account/EmailLogin.cshtml", model);
        }

        var loginResult = await _userManager.AddLoginAsync(user,
            new UserLoginInfo(provider, providerUserId, provider));
        if (!loginResult.Succeeded)
        {
            foreach (var error in loginResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return View("~/Views/Account/EmailLogin.cshtml", model);
        }

        var isuser = new IdentityServerUser(user.Id)
        {
            DisplayName = user.UserName,
            IdentityProvider = provider
        };
        await HttpContext.SignInAsync(isuser);

        HttpContext.Session.Remove("ext_provider");
        HttpContext.Session.Remove("ext_providerUserId");
        HttpContext.Session.Remove("ext_email");
        HttpContext.Session.Remove("ext_returnUrl");

        var returnUrl = model.ReturnUrl ?? "~/";
        if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return Redirect("~/");
    }

    private async Task<(ApplicationUser user, string provider, string providerUserId, IEnumerable<Claim> claims)>
        FindUserFromExternalProviderAsync(AuthenticateResult result)
    {
        var externalUser = result.Principal;
        var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                          externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                          throw new Exception("Unknown userid");

        var claims = externalUser.Claims.ToList();
        claims.Remove(userIdClaim);

        var provider = result.Properties.Items["scheme"];
        var providerUserId = userIdClaim.Value;

        var user = await _userManager.FindByLoginAsync(provider, providerUserId);

        return (user, provider, providerUserId, claims);
    }
    private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
    {
        var filtered = new List<Claim>();
        var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
            claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        if (name != null)
        {
            filtered.Add(new Claim(JwtClaimTypes.Name, name));
        }
        else
        {
            var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
            if (first != null && last != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
            }
            else if (first != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, first));
            }
            else if (last != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, last));
            }
        }

        var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
           claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (email != null)
        {
            filtered.Add(new Claim(JwtClaimTypes.Email, email));
        }

        var user = new ApplicationUser
        {
            UserName = Guid.NewGuid().ToString(),
        };
        var identityResult = await _userManager.CreateAsync(user);
        if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

        if (filtered.Any())
        {
            identityResult = await _userManager.AddClaimsAsync(user, filtered);
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
        }

        identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
        if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

        return user;
    }

    private void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
    {
        var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        }

        var idToken = externalResult.Properties.GetTokenValue("id_token");
        if (idToken != null)
        {
            localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
        }
    }
}
