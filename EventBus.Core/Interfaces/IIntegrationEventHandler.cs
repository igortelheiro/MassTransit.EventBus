using System.Threading.Tasks;
using EventBus.Core.Events;

namespace EventBus.Core.Interfaces
{
    public interface IIntegrationEventHandler<in TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }
}
