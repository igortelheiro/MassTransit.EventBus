using System;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Core.Events;
using EventBus.Core.Interfaces;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQEventBus
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
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await _bus.Publish(@event, source.Token);
        }


        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _bus.ConnectReceiveEndpoint(typeof(TH).Name, config =>
            {
                config.UseMessageRetry(r =>
                    {
                        r.Ignore<ArgumentException>();
                        r.Ignore<TimeoutException>();
                        r.Immediate(5);
                        r.Interval(3, TimeSpan.FromSeconds(5));
                    });

                var handler = _serviceProvider.GetRequiredService<IIntegrationEventHandler<T>>();
                var consumer = new MassTransitConsumer<T>(handler);
                config.Consumer(consumer.GetType(), _ => consumer);
            });
        }
    }
}
