namespace SmartDiscount.Catalog.API.CatalogIntegrationEvents.Events;

public record OrderStatusChangedToAwaitingValidationIntegrationEvent(int OrderId, IEnumerable<OrderStockItem> OrderStockItems) : IntegrationEvent;