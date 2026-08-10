namespace SmartDiscount.Basket.API.IntegrationEvents.Events;
public record OrderStartedIntegrationEvent(string UserId) : IntegrationEvent;