using EventBus.Core.Events;
using EventBus.Core.Interfaces;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.EventBus.Core;

public sealed class MassTransitEventBus : IEventBus
{
    private readonly IBus _bus;
    private readonly IServiceProvider _serviceProvider;

    public MassTransitEventBus(IBus bus, IServiceProvider serviceProvider)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }


    public async Task PublishAsync(dynamic @event, CancellationToken cancellationToken) =>
        await _bus.Publish(@event, cancellationToken).ConfigureAwait(false);


    public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T> =>
            _bus.ConnectReceiveEndpoint(config =>
            {
                config.UseMessageRetry(r =>
                    {
                        r.Ignore<ArgumentException>();
                        r.Immediate(3);
                    });

                var handler = _serviceProvider.GetRequiredService<IIntegrationEventHandler<T>>();
                var consumer = new MassTransitConsumer<T>(handler);
                config.Consumer(consumer.GetType(), _ => consumer);
            });
}