using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SmartDiscount.Identity.API.Models;
using SmartDiscount.Identity.API.Models.PasswordViewModels;
using SmartDiscount.Identity.API.Services;

namespace IdentityServerHost.Quickstart.UI;

[SecurityHeaders]
[AllowAnonymous]
public class PasswordController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IMemoryCache _cache;

    public PasswordController(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IMemoryCache cache)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult ForgotPassword(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null && await _userManager.HasPasswordAsync(user))
        {
            var code = new Random().Next(1000, 9999).ToString();
            _cache.Set("reset_" + model.Email.ToLower(), code, TimeSpan.FromMinutes(10));

            var body = $@"
                <div style='font-family:Arial,sans-serif;padding:20px'>
                    <h2 style='color:#1a2b4a'>SmartDiscount — Reset your password</h2>
                    <p>Your 4-digit reset code is:</p>
                    <p style='font-size:32px;font-weight:bold;color:#d4a017;letter-spacing:8px'>{code}</p>
                    <p>This code expires in 10 minutes.</p>
                </div>";

            await _emailSender.SendEmailAsync(user.Email, "Your SmartDiscount reset code", body);
        }

        TempData["ResetEmail"] = model.Email;
        TempData["ReturnUrl"] = returnUrl;   
        return RedirectToAction("VerifyCode");
    }

    [HttpGet]
    public IActionResult VerifyCode()
    {
        var email = TempData["ResetEmail"] as string;
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("ForgotPassword");

        TempData.Keep("ResetEmail");
        TempData.Keep("ReturnUrl");  

        var vm = new VerifyCodeViewModel { Email = email };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult VerifyCode(VerifyCodeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData.Keep("ResetEmail");
            TempData.Keep("ReturnUrl");
            return View(model);
        }

        var cachedCode = _cache.Get<string>("reset_" + model.Email.ToLower());

        if (cachedCode == null)
        {
            ModelState.AddModelError(string.Empty, "The code has expired. Please request a new one.");
            TempData.Keep("ResetEmail");
            TempData.Keep("ReturnUrl");
            return View(model);
        }

        if (cachedCode != model.Code)
        {
            ModelState.AddModelError(string.Empty, "Invalid code. Please try again.");
            TempData.Keep("ResetEmail");
            TempData.Keep("ReturnUrl");
            return View(model);
        }

        TempData["ResetEmail"] = model.Email;
        TempData["CodeVerified"] = "true";
        TempData.Keep("ReturnUrl");   
        return RedirectToAction("ResetPassword");
    }


    [HttpGet]
    public IActionResult ResetPassword()
    {
        var email = TempData["ResetEmail"] as string;
        var verified = TempData["CodeVerified"] as string;

        if (string.IsNullOrEmpty(email) || verified != "true")
            return RedirectToAction("ForgotPassword");

        TempData.Keep("ResetEmail");
        TempData.Keep("CodeVerified");
        TempData.Keep("ReturnUrl"); 

        var vm = new ResetPasswordViewModel { Email = email };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData.Keep("ResetEmail");
            TempData.Keep("CodeVerified");
            TempData.Keep("ReturnUrl");
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return RedirectToAction("Login", "Account");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            TempData.Keep("ResetEmail");
            TempData.Keep("CodeVerified");
            TempData.Keep("ReturnUrl");
            return View(model);
        }

        _cache.Remove("reset_" + model.Email.ToLower());
        var returnUrl = TempData["ReturnUrl"] as string;
        if (!string.IsNullOrEmpty(returnUrl))
            return RedirectToAction("Login", "Account", new { returnUrl });

        return RedirectToAction("Login", "Account");
    }
}