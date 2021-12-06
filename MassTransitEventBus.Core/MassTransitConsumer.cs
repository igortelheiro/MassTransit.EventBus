using EventBus.Core.Events;
using EventBus.Core.Interfaces;

namespace MassTransit.EventBus.Core;

public sealed class MassTransitConsumer<TMessage> : IConsumer<TMessage>
    where TMessage : IntegrationEvent
{
    private readonly IIntegrationEventHandler<TMessage> _handler;

    public MassTransitConsumer(IIntegrationEventHandler<TMessage> handler) =>
        _handler = handler ?? throw new ArgumentNullException($"Nenhum handler foi encontrado para: {typeof(TMessage).Name}");


    public async Task Consume(ConsumeContext<TMessage> context) =>
        await _handler.Handle(context.Message).ConfigureAwait(false);
}
