using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MGR.EventBus.Events;
using MGR.EventBus.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MGR.RabbitMQEventBus
{
    public class MassTransitEventBus : IEventBus
    {
        #region Initialize
        private readonly IBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public MassTransitEventBus(IBus bus, IServiceProvider serviceProvider)
        {
            _bus = bus;
            _serviceProvider = serviceProvider;
        }
        #endregion


        public async Task Publish<TEvent>(TEvent @event)
            where TEvent : IntegrationEvent
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await _bus.Publish(@event, source.Token);
        }


        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _bus.ConnectReceiveEndpoint(typeof(TH).Name, e =>
            {
                e.UseMessageRetry(r =>
                    {
                        r.Ignore<ArgumentException>();
                        r.Ignore<TimeoutException>();
                        r.Immediate(5);
                    });

                var handler = _serviceProvider.GetRequiredService<IIntegrationEventHandler<T>>();
                if (handler == null)
                {
                    throw new ArgumentException($"Nenhum handler registrado para: {typeof(T).Name}");
                }
                var consumer = new MassTransitConsumer<T>(handler);
                e.Consumer(consumer.GetType(), _ => consumer);
            });
        }
    }
}
