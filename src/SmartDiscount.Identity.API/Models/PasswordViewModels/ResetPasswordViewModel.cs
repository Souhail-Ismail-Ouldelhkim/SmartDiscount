using System.ComponentModel.DataAnnotations;

namespace SmartDiscount.Identity.API.Models.PasswordViewModels
{
    public record ResetPasswordViewModel
    {
        public string Email { get; init; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; init; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; init; }
    }
}