using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Database;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Initializers;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.System.Settings.Initializers;
using Mint.Database.Entities.UserInteractive.Stats;
using Mint.Database.Entities.UserInteractive.Stats.Initializers;
using Mint.Database.Entities.Users;

namespace Mint.UnitTests.AppServices.System.Fixtures.Seeding;

/// <summary>
/// Seeder for referral command test data using EF Core entities.
/// </summary>
public static class ReferralCommandSeeder
{
    /// <summary>
    /// Seeds the database with scenarios, steps, buttons, users, accounts, stats and system settings using EF Core entities.
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
                Id = 2,
                ExternalUserId = 1002,
                SystemType = 1,
                FirstName = "Alice",
                LastName = "Smith",
                UserName = "alice.smith",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 3,
                ExternalUserId = 1003,
                SystemType = 1,
                FirstName = "Bob",
                LastName = "Johnson",
                UserName = "bob.johnson",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 4,
                ExternalUserId = 1004,
                SystemType = 1,
                FirstName = "Charlie",
                LastName = "Brown",
                UserName = "charlie.brown",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            });

        context.Accounts.AddRange(
            new AccountEntity
            {
                Id = 2,
                UserId = 2,
                Balance = 1500.50m,
                CreatedAt = DateTimeOffset.UtcNow,
                LastTransactionDate = DateTimeOffset.UtcNow,
                Status = AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 3,
                UserId = 3,
                Balance = 3200.00m,
                CreatedAt = DateTimeOffset.UtcNow,
                LastTransactionDate = DateTimeOffset.UtcNow,
                Status = AccountStatus.Active
            });

        context.UserStats.AddRange(
            new UserStatsEntity
            {
                Id = 1,
                UserId = 2,
                RankPoints = 150,
                TotalWins = 10,
                TotalLosses = 5,
                ReferralCount = 2
            },
            new UserStatsEntity
            {
                Id = 2,
                UserId = 3,
                RankPoints = 75,
                TotalWins = 5,
                TotalLosses = 8,
                ReferralCount = 0
            });

        context.SystemSettings.AddRange(new SettingsInitializer().Get());

        context.SaveChanges();
    }
}
