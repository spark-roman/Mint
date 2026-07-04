using Microsoft.EntityFrameworkCore;
using Mint.Database;

namespace Mint.Bot.Infrastructure;

/// <summary>
/// Migrations extension methods.
/// </summary>
public static class MigrationsExtensions
{
    /// <summary>
    /// Apply database migrations
    /// </summary>
    /// <param name="app">Web application</param>
    public static async Task ApplyMigrations(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<MintDbContext>();

        context.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

        await context.Database.MigrateAsync();
    }
}
