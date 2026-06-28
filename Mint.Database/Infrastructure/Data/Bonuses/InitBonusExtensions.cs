using Microsoft.EntityFrameworkCore;
using Mint.Database.Entities.UserInteractive.Bonuses;
using Mint.Database.Entities.UserInteractive.Bonuses.Initializers;

namespace Mint.Database.Infrastructure.Data.Bonuses;

/// <summary>
/// Extension methods for initializing bonus data.
/// </summary>
public static class InitBonusExtensions
{
    /// <summary>
    /// Initializes bonus data in the database.
    /// </summary>
    /// <param name="modelBuilder">Model builder.</param>
    public static void InitBonusData(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<BonusTypeEntity>().HasData(new BonusTypeInitializer().Get());
    }
}
