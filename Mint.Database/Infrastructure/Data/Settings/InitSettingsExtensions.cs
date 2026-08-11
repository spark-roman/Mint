using Microsoft.EntityFrameworkCore;
using Mint.Database.Entities.System.Settings;
using Mint.Database.Entities.System.Settings.Initializers;

namespace Mint.Database.Infrastructure.Data.Settings;

/// <summary>
/// Extension methods for initializing settings data.
/// </summary>
public static class InitSettingsExtensions
{
    /// <summary>
    /// Initialize settings data in the database.
    /// </summary>
    /// <param name="modelBuilder">Model builder.</param>
    public static void InitSettingsData(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<SystemSettingEntity>().HasData(new SettingsInitializer().Get());
    }
}
