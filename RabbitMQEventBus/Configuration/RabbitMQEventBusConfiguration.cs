using System;
using System.Threading;
using EventBus.Core.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQEventBus.Configuration
{
    public static class RabbitMQEventBusConfiguration
    {
        public static void ConfigureRabbitMQEventBus(this IServiceCollection services)
        {
            var busControl = ConfigureEventBusFactory();

            services.AddSingleton(busControl);
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IEventBus, MassTransitEventBus>();

            //TODO: Use IHostedService to control bus lifetime
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            busControl.StartAsync(source.Token);
        }


        private static IBusControl ConfigureEventBusFactory() =>
            Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                        h.RequestedChannelMax(30);
                        h.RequestedConnectionTimeout(TimeSpan.FromSeconds(30));
                    });
                });
    }
}
