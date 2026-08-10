namespace SmartDiscount.Wishlist.API.Model
{
    public class WishlistItem 
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public int ProductId { get; set; }
        public DateTime DateAdded { get; set; }
        public WishlistStatus Status { get; set; } = WishlistStatus.Active;
        public DateTime? DateStatusChanged { get; set; }
    }
}
