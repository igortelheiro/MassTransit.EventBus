using EventBus.Core.Interfaces;
using IntegrationEventLogEF.Configuration;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace RabbitMQEventBus.Configuration;

public static class RabbitMQEventBusConfiguration
{
    public static void ConfigureRabbitMQEventBus(this IServiceCollection services)
    {
        services.ConfigureIntegrationEventLog();

        var busControl = BuildEventBus();

        services.AddScoped<IEventBus>(provider => new MassTransitEventBus(busControl, provider));

        //TODO: Use IHostedService to control bus lifetime
        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token;
        busControl.StartAsync(cancellationToken);
    }


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
