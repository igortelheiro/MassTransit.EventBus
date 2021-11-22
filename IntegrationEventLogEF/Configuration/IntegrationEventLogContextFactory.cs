using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IntegrationEventLogEF.Configuration;

public class IntegrationEventLogContextFactory : IDesignTimeDbContextFactory<IntegrationEventLogContext>
{
    public IntegrationEventLogContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogContext>();
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=DbTest");

        return new IntegrationEventLogContext(optionsBuilder.Options);
    }
}