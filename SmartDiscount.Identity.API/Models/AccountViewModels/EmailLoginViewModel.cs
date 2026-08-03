using System.ComponentModel.DataAnnotations;

namespace SmartDiscount.Identity.API.Models.AccountViewModels
{
    public record EmailLoginViewModel
    {
        public string Email { get; init; }

        [Required]
        [Display(Name = "First name")]
        public string Name { get; init; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; init; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; init; }

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

        public string ReturnUrl { get; init; }
    }
}