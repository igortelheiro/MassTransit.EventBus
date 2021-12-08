using IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IntegrationEventLogEF.Configuration;

public static class IntegrationEventLogConfiguration
{
    /// <summary>
    /// Adiciona a interface IIntegrationEventLogService.
    /// É necessário configurar uma ConnectionString chamada "DbConnection" em appsettings
    /// </summary>
    public static void ConfigureIntegrationEventLog(this IServiceCollection services, IConfiguration configuration)
    {
        //TODO: Create IntegrationEventLog table when initializing

        var connectionString = configuration.GetConnectionString("DbConnection")
            ?? throw new ArgumentNullException("ConnectionStrings.DbConnection não encontrado no appsettings");

        services.AddDbContext<IntegrationEventLogContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();
    }
}