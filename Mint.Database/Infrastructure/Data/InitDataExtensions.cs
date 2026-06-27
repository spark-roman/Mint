using Microsoft.EntityFrameworkCore;
using Mint.Database.Infrastructure.Data.Bonuses;
using Mint.Database.Infrastructure.Data.Promts;

namespace Mint.Database.Infrastructure.Data;

/// <summary>
/// Extension methods for initializing data.
/// </summary>
public static class InitDataExtensions
{
    /// <summary>
    /// Initializes data in the database.
    /// </summary>
    /// <param name="modelBuilder">Model builder.</param>
    public static void InitData(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.InitBonusData();
        modelBuilder.InitPromtsData();
    }
}
