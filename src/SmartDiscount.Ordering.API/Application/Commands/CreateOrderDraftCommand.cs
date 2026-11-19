namespace SmartDiscount.Ordering.API.Application.Commands;

using SmartDiscount.Ordering.API.Application.Models;

public record CreateOrderDraftCommand(string BuyerId, IEnumerable<BasketItem> Items) : IRequest<OrderDraftDTO>;
