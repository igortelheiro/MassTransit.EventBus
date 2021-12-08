using System.Threading;
using System.Threading.Tasks;
using EventBus.Core.Events;

namespace EventBus.Core.Interfaces
{
    public interface IEventBus
    {
        //TODO: implementar params (usando MassTransit.TransactionalEnlistmentBus)
        //Task PublishAsync<TEvent>(params TEvent[] @event)
        Task PublishAsync(dynamic @event, CancellationToken cancellationToken);

        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}
