using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IntegrationEventLogEF.Configuration;

public class IntegrationEventLogContextFactory : IDesignTimeDbContextFactory<IntegrationEventLogContext>
{
    // Classe utilizada para configuração das migrations utilizando EF Tools
    public IntegrationEventLogContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=LoginDbTest;Trusted_Connection=True");

        return new IntegrationEventLogContext(optionsBuilder.Options);
    }
}