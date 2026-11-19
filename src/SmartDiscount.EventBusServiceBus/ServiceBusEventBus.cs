using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartDiscount.EventBus.Abstractions;
using SmartDiscount.EventBus.Events;
using System.Diagnostics;
using System.Text.Json;

namespace SmartDiscount.EventBusServiceBus;

public sealed class ServiceBusEventBus(
    ILogger<ServiceBusEventBus> logger,
    IServiceProvider serviceProvider,
    IOptions<EventBusServiceBusOptions> options,
    IOptions<EventBusSubscriptionInfo> subscriptionOptions,
    ServiceBusClient serviceBusClient) : IEventBus, IAsyncDisposable, IHostedService
{
    private const string TopicName = "smartdiscount-event-bus";

    private readonly string _subscriptionName = options.Value.SubscriptionClientName;
    private readonly EventBusSubscriptionInfo _subscriptionInfo = subscriptionOptions.Value;

    private ServiceBusSender _sender;
    private ServiceBusProcessor _processor;

    public async Task PublishAsync(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name;

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Publishing event to Service Bus: {EventId} ({EventName})", @event.Id, eventName);
        }

        var body = SerializeMessage(@event);

        var message = new ServiceBusMessage(body)
        {
            MessageId = @event.Id.ToString(),
            Subject = eventName,
            ContentType = "application/json"
        };

        _sender ??= serviceBusClient.CreateSender(TopicName);

        await _sender.SendMessageAsync(message);
    }

    public async ValueTask DisposeAsync()
    {
        if (_processor is not null)
        {
            await _processor.DisposeAsync();
        }
        if (_sender is not null)
        {
            await _sender.DisposeAsync();
        }
    }

    private async Task OnMessageReceived(ProcessMessageEventArgs args)
    {
        var eventName = args.Message.Subject;
        var message = args.Message.Body.ToString();

        try
        {
            await ProcessEvent(eventName, message);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error Processing message \"{Message}\"", message);
        }
        await args.CompleteMessageAsync(args.Message);
    }

    private Task OnProcessError(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Error handling Service Bus message");
        return Task.CompletedTask;
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("Processing Service Bus event: {EventName}", eventName);
        }

        await using var scope = serviceProvider.CreateAsyncScope();

        if (!_subscriptionInfo.EventTypes.TryGetValue(eventName, out var eventType))
        {
            logger.LogWarning("Unable to resolve event type for event name {EventName}", eventName);
            return;
        }

        var integrationEvent = DeserializeMessage(message, eventType);

        foreach (var handler in scope.ServiceProvider.GetKeyedServices<IIntegrationEventHandler>(eventType))
        {
            await handler.Handle(integrationEvent);
        }
    }

    private IntegrationEvent DeserializeMessage(string message, Type eventType)
    {
        return JsonSerializer.Deserialize(message, eventType, _subscriptionInfo.JsonSerializerOptions) as IntegrationEvent;
    }

    private byte[] SerializeMessage(IntegrationEvent @event)
    {
        return JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), _subscriptionInfo.JsonSerializerOptions);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Service Bus processor for subscription {SubscriptionName}", _subscriptionName);

        _processor = serviceBusClient.CreateProcessor(TopicName, _subscriptionName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 1
        });

        _processor.ProcessMessageAsync += OnMessageReceived;
        _processor.ProcessErrorAsync += OnProcessError;

        await _processor.StartProcessingAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor is not null)
        {
            await _processor.StopProcessingAsync(cancellationToken);
        }
    }
}