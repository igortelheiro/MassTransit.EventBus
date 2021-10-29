using MGR.EventBus.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MGR.EventBus.Configuration
{
    public static class EventBusConfiguration
    {
        public static void ConfigureEventBusSubscriptionManager(this IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
    }
}
