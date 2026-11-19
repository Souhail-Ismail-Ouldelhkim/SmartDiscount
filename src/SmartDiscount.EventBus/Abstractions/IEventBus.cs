using System.Threading.Tasks;

namespace SmartDiscount.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent @event);
}
