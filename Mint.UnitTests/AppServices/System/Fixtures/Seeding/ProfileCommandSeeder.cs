using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Database;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Initializers;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Stats;
using Mint.Database.Entities.UserInteractive.Stats.Initializers;
using Mint.Database.Entities.Users;

namespace Mint.UnitTests.AppServices.System.Fixtures.Seeding;

/// <summary>
/// Seeder for profile command test data using EF Core entities.
/// </summary>
public static class ProfileCommandSeeder
{
    /// <summary>
    /// Seeds the database with scenarios, steps, buttons, users, accounts and stats using EF Core entities.
    /// </summary>
    /// <param name="context">Database context to seed.</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var botInitializer = new BotInitializer();
        var rankInitializer = new RankConfigInitializer();

        var stepTypes = botInitializer.GetStepTypes();
        var scenarios = botInitializer.GetScenarios();
        var steps = botInitializer.GetSteps();
        var buttons = botInitializer.GetButtons();
        var rankConfigs = rankInitializer.Get();

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

        foreach (var rankConfig in rankConfigs)
        {
            context.RankConfigs.Add(rankConfig);
        }

        context.Users.AddRange(
            new UserEntity
            {
                Id = 1,
                ExternalUserId = 1001,
                SystemType = 1,
                FirstName = "Alice",
                LastName = "Smith",
                UserName = "alice.smith",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 2,
                ExternalUserId = 1002,
                SystemType = 1,
                FirstName = "Bob",
                LastName = "Johnson",
                UserName = "bob.johnson",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            });

        context.Accounts.AddRange(
            new AccountEntity
            {
                Id = 1,
                UserId = 1,
                Balance = 1500.50m,
                CreatedAt = DateTimeOffset.UtcNow,
                LastTransactionDate = DateTimeOffset.UtcNow,
                Status = AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 2,
                UserId = 2,
                Balance = 3200.00m,
                CreatedAt = DateTimeOffset.UtcNow,
                LastTransactionDate = DateTimeOffset.UtcNow,
                Status = AccountStatus.Active
            });

        context.UserStats.AddRange(
            new UserStatsEntity
            {
                Id = 1,
                UserId = 1,
                RankPoints = 150,
                TotalWins = 10,
                TotalLosses = 5,
                ReferralCount = 2,
                ReferralEarnings = 1000.00m
            },
            new UserStatsEntity
            {
                Id = 2,
                UserId = 2,
                RankPoints = 75,
                TotalWins = 5,
                TotalLosses = 8,
                ReferralCount = 0,
                ReferralEarnings = 0.00m
            });

        context.SaveChanges();
    }
}
