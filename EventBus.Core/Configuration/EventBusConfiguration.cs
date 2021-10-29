using EventBus.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.Core.Configuration
{
    public static class EventBusConfiguration
    {
        public static void ConfigureEventBusSubscriptionManager(this IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
    }
}
