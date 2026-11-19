using SmartDiscount.EventBus.Events;

namespace SmartDiscount.Webhooks.API.IntegrationEvents;

public record OrderStatusChangedToShippedIntegrationEvent(int OrderId, string OrderStatus, string BuyerName) : IntegrationEvent;