namespace SmartDiscount.Catalog.API.CatalogIntegrationEvents.Events;

public record OrderStockConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;