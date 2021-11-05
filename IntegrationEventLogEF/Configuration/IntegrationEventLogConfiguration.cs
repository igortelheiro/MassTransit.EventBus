using IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace IntegrationEventLogEF.Configuration
{
    public static class IntegrationEventLogConfiguration
    {
        public static void ConfigureIntegrationEventLog(this IServiceCollection services)
        {
            //TODO: Create IntegrationEventLog table when initializing

            using var serviceProvider = services.BuildServiceProvider();
            var dbConnection = serviceProvider.GetRequiredService<DbConnection>().ConnectionString;

            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseSqlServer(dbConnection);
            });

            services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>(provider =>
            {
                var connection = provider.GetRequiredService<DbConnection>();
                return new IntegrationEventLogService(connection);
            });
        }
    }
}
