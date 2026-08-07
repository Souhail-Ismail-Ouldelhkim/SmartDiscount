using SmartDiscount.Notification.API.IntegrationEvents.Events;
using SmartDiscount.Notification.API.IntegrationEvents.EventHandling;
using SmartDiscount.Notification.API.Services;

namespace SmartDiscount.Notification.API.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddRabbitMqEventBus("eventbus")
               .AddSubscription<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();

        builder.Services.AddHttpClient<IdentityClient>(client =>
        {
            client.BaseAddress = new Uri("https://identity-api");
        });
        builder.Services.AddHttpClient<OrderingClient>(client =>
        {
            client.BaseAddress = new Uri("https://ordering-api");
        });
    }


}