using Microsoft.EntityFrameworkCore;
using Mint.Database.Entities.System;
using Mint.Database.Entities.System.Initializers;
using Mint.Database.Entities.UserInteractive.UserCategories;
using Mint.Database.Entities.UserInteractive.UserCategories.Initializers;

namespace Mint.Database.Infrastructure.Data.Promts;

/// <summary>
/// Extension methods for initializing promts data.
/// </summary>
public static class InitPromtsExtensions
{
    /// <summary>
    /// Initialize promt data in the database.
    /// </summary>
    /// <param name="modelBuilder">Model builder.</param>
    public static void InitPromtsData(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<AiPromptEntity>().HasData(new PromptInitializer().Get());
        modelBuilder.Entity<CategoryEntity>().HasData(new CategoryInitializer().Get());
    }
}