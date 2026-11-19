using Microsoft.Extensions.DependencyInjection;
using SmartDiscount.EventBusServiceBus;

namespace Microsoft.Extensions.Hosting;

public static class ServiceBusDependencyInjectionExtensions
{
    private const string SectionName = "EventBus";

    public static IEventBusBuilder AddServiceBusEventBus(this IHostApplicationBuilder builder, string connectionName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.AddAzureServiceBusClient(connectionName);
        builder.Services.Configure<EventBusServiceBusOptions>(builder.Configuration.GetSection(SectionName));
        builder.Services.AddSingleton<IEventBus, ServiceBusEventBus>();
        builder.Services.AddSingleton<IHostedService>(sp => (ServiceBusEventBus)sp.GetRequiredService<IEventBus>());
        return new EventBusBuilder(builder.Services);
    }

    private class EventBusBuilder(IServiceCollection services) : IEventBusBuilder
    {
        public IServiceCollection Services => services;
    }
}