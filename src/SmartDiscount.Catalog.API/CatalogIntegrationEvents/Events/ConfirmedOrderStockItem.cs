namespace SmartDiscount.Catalog.API.CatalogIntegrationEvents.Events;
public record ConfirmedOrderStockItem(int ProductId, bool HasStock);