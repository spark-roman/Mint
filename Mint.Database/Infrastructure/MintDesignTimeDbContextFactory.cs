using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mint.Database.Infrastructure;

/// <inheritdoc/>
public class MintDesignTimeDbContextFactory : IDesignTimeDbContextFactory<MintDbContext>
{
    /// <inheritdoc/>
    public MintDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<MintDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseNpgsql(connectionString);

        return new MintDbContext(optionsBuilder.Options);
    }
}
