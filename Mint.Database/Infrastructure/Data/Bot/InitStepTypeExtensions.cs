using Microsoft.EntityFrameworkCore;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Initializers;

namespace Mint.Database.Infrastructure.Data.Bot;

/// <summary>
/// Extension methods for initializing step type data.
/// </summary>
public static class InitStepTypeExtensions
{
    /// <summary>
    /// Initialize step type data in the database.
    /// </summary>
    /// <param name="modelBuilder">Model builder.</param>
    public static void InitStepTypeData(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<StepTypeEntity>().HasData(new StepTypeInitializer().Get());
    }
}
