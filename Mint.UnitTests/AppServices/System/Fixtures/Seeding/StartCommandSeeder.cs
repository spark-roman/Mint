using System.Globalization;
using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Database;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Initializers;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Users;

namespace Mint.UnitTests.AppServices.System.Fixtures.Seeding;

/// <summary>
/// Seeder for start command test data using EF Core entities.
/// </summary>
public static class StartCommandSeeder
{
    /// <summary>
    /// Seeds the database with scenarios, steps, buttons and test users using EF Core entities.
    /// </summary>
    /// <param name="context">Database context to seed.</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var botInitializer = new BotInitializer();

        var stepTypes = botInitializer.GetStepTypes();
        var scenarios = botInitializer.GetScenarios();
        var steps = botInitializer.GetSteps();
        var buttons = botInitializer.GetButtons();

        foreach (var stepType in stepTypes)
        {
            context.StepTypes.Add(stepType);
        }

        foreach (var scenario in scenarios)
        {
            context.Scenarios.Add(scenario);
        }

        foreach (var step in steps)
        {
            context.Steps.Add(step);
        }

        foreach (var button in buttons)
        {
            context.Buttons.Add(button);
        }

        context.Accounts.Add(new AccountEntity
        {
            Id = 1,
            UserId = 0,
            Balance = 999999999.00m,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = AccountStatus.Active
        });

        context.Users.AddRange(
            new UserEntity
            {
                Id = 1,
                ExternalUserId = 12345,
                SystemType = 1,
                FirstName = "Test",
                LastName = "User",
                UserName = "testuser",
                CreatedAt = DateTimeOffset.Parse("1987-01-20T00:00:00.000Z", CultureInfo.InvariantCulture),
                Status = 1
            },
            new UserEntity
            {
                Id = 2,
                ExternalUserId = 999,
                SystemType = 1,
                FirstName = "Alice",
                LastName = "Bob",
                UserName = "alice_bob",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 3,
                ExternalUserId = 777,
                SystemType = 1,
                FirstName = "Test",
                LastName = "User",
                UserName = "testuser",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 4,
                ExternalUserId = 42,
                SystemType = 1,
                FirstName = "Test",
                LastName = "User",
                UserName = "testuser",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            });

        context.SaveChanges();
    }
}
