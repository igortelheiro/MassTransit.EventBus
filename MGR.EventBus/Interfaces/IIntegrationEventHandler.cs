using System.Threading.Tasks;
using MGR.EventBus.Events;

namespace MGR.EventBus.Interfaces
{
    public interface IIntegrationEventHandler<in TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }
}
