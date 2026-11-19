using System.ComponentModel.DataAnnotations;

namespace SmartDiscount.Identity.API.Models.AccountViewModels
{
    public record RegisterViewModel
    {
        [Required]
        [Display(Name = "First name")]
        public string Name { get; init; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; init; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; init; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; init; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; init; }

        [Required]
        [Display(Name = "Street")]
        public string Street { get; init; }

        [Required]
        [Display(Name = "City")]
        public string City { get; init; }

        [Required]
        [Display(Name = "State")]
        public string State { get; init; }

        [Required]
        [Display(Name = "Zip code")]
        public string ZipCode { get; init; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; init; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; init; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; init; }

        [Required]
        [Display(Name = "Cardholder name")]
        public string CardHolderName { get; init; }

        [Required]
        [Display(Name = "Card number")]
        public string CardNumber { get; init; }

        [Required]
        [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Expiration should match MM/YY")]
        [Display(Name = "Expiration")]
        public string Expiration { get; init; }

        [Required]
        [Display(Name = "Security number")]
        public string SecurityNumber { get; init; }

        [Required]
        [Display(Name = "Card type")]
        public int CardType { get; init; }

        public string? ReturnUrl { get; init; }
    }
}