using System.Threading.Tasks;
using MGR.EventBus.Events;

namespace MGR.EventBus.Interfaces
{
    public interface IEventBus
    {
        Task Publish<TEvent>(TEvent @event)
            where TEvent : IntegrationEvent;

        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}
