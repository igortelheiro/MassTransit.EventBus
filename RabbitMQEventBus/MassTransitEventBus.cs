using EventBus.Core.Interfaces;
using GreenPipes;
using IntegrationEventLogEF;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace RabbitMQEventBus
{
    public class MassTransitEventBus : LoggingEventBus
    {
        private readonly IBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public MassTransitEventBus(IBus bus, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }


        public override async Task PublishAsync<TEvent>(TEvent @event) =>
            await LogAndPublishAsync(@event, async cancellationToken =>
            {
                await _bus.Publish(@event, cancellationToken);
            });


        public override void Subscribe<T, TH>() =>
            _bus.ConnectReceiveEndpoint(typeof(TH).Name, config =>
            {
                config.UseMessageRetry(r =>
                    {
                        r.Ignore<ArgumentException>();
                        r.Ignore<TimeoutException>();
                        r.Immediate(3);
                    });

                var handler = _serviceProvider.GetRequiredService<IIntegrationEventHandler<T>>();
                var consumer = new MassTransitConsumer<T>(handler);
                config.Consumer(consumer.GetType(), _ => consumer);
            });
    }
}
