using System.ComponentModel.DataAnnotations;

namespace SmartDiscount.Identity.API.Models.PasswordViewModels;

public record ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; init; }
}