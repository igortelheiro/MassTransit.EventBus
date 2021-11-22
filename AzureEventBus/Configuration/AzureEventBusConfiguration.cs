using EventBus.Core.Interfaces;
using MassTransit;
using MassTransit.EventBus.Core;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQEventBus.Configuration;

public static class AzureEventBusConfiguration
{
    public static async Task ConfigureAzureEventBus(this IServiceCollection services)
    {
        //services.ConfigureIntegrationEventLog();

        var busControl = BuildEventBus();

        services.AddScoped<IEventBus>(provider => new MassTransitEventBus(busControl, provider));

        //TODO: Use IHostedService to control bus lifetime
        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token;
        await busControl.StartAsync(cancellationToken).ConfigureAwait(false);
    }


    private static IBusControl BuildEventBus() =>
        Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.Host("Endpoint=sb://mgr-masstransit.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=bmmu9YRn9Ms+YTsMDI0TDl92bF/UHF5vnG0gVu0Pcgg=", h =>
                {
                });
            });
}
