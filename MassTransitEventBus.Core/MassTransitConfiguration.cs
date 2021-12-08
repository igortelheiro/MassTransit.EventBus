using EventBus.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.EventBus.Core
{
    public static class MassTransitConfiguration
    {
        public static async Task ConfigureMassTransit(this IServiceCollection services, IBusControl busControl)
        {
            services.AddScoped<IEventBus>(provider => new MassTransitEventBus(busControl, provider));

            try
            {
                var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token;
                await busControl.StartAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await busControl.StopAsync().ConfigureAwait(false);
            }
        }
    }
}
