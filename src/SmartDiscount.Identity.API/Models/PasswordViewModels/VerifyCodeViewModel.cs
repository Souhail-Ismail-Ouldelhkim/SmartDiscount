using System.ComponentModel.DataAnnotations;

namespace SmartDiscount.Identity.API.Models.PasswordViewModels
{
    public record VerifyCodeViewModel
    {
        public string Email { get; init; }

        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "The code must be 4 digits.")]
        public string Code { get; init; }
    }
}