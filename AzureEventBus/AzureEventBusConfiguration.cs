using MassTransit;
using MassTransit.EventBus.Core;
using Microsoft.Extensions.DependencyInjection;

namespace AzureEventBus;

public static class AzureEventBusConfiguration
{
    /// <summary>
    /// Adiciona a interface IEventBus utilizando AzureServiceBus
    /// </summary>
    public static Task ConfigureAzureEventBus(this IServiceCollection services) =>
        services.ConfigureMassTransit(BuildEventBus());


    private static IBusControl BuildEventBus() =>
        Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.Host("Endpoint=sb://mgr-masstransit.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=bmmu9YRn9Ms+YTsMDI0TDl92bF/UHF5vnG0gVu0Pcgg=");
            });
}
