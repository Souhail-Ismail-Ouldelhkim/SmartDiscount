using SmartDiscount.EventBus.Events;

namespace SmartDiscount.Notification.API.IntegrationEvents.Events;

public record OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public string BuyerName { get; }
    public string BuyerIdentityGuid { get; }
    public IEnumerable<OrderStockItem> OrderStockItems { get; }

    public OrderStatusChangedToPaidIntegrationEvent(int orderId,
        string buyerName, string buyerIdentityGuid,
        IEnumerable<OrderStockItem> orderStockItems)
    {
        OrderId = orderId;
        BuyerName = buyerName;
        BuyerIdentityGuid = buyerIdentityGuid;
        OrderStockItems = orderStockItems;
    }
}