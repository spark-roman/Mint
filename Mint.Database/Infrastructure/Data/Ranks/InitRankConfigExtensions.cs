using Microsoft.EntityFrameworkCore;
using Mint.Database.Entities.UserInteractive.Stats;
using Mint.Database.Entities.UserInteractive.Stats.Initializers;

namespace Mint.Database.Infrastructure.Data.Ranks;

/// <summary>
/// Extension methods for initializing rank config data.
/// </summary>
public static class InitRankConfigExtensions
{
    /// <summary>
    /// Initialize rank config data in the database.
    /// </summary>
    /// <param name="modelBuilder">Model builder.</param>
    public static void InitRankConfigData(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<RankConfigEntity>().HasData(new RankConfigInitializer().Get());
    }
}
