using MassTransit;
using MassTransit.EventBus.Core;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQEventBus.Configuration;

public static class RabbitMQEventBusConfiguration
{
    /// <summary>
    /// Adiciona a interface IEventBus utilizando RabbitMQ
    /// </summary>
    public static Task ConfigureRabbitMQEventBus(this IServiceCollection services) =>
        services.ConfigureMassTransit(BuildEventBus());


    private static IBusControl BuildEventBus() =>
        Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                    h.RequestedChannelMax(30);
                    h.RequestedConnectionTimeout(TimeSpan.FromSeconds(20));
                });
            });
}
