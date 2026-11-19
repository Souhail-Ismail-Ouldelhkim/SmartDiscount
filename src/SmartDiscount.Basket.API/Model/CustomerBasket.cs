using System.Collections.Generic;
namespace SmartDiscount.Basket.API.Model
{
    public class CustomerBasket
    {
        public string BuyerId { get; set; }
        public List<BasketItem> Items { get; set; } = [];
        public string PromoCode { get; set; }          
        public decimal DiscountAmount { get; set; }    
        public CustomerBasket() { }
        public CustomerBasket(string customerId)
        {
            BuyerId = customerId;
        }
    }
}