using System.Globalization;
using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Database;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Initializers;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Bonuses;
using Mint.Database.Entities.Users;

namespace Mint.UnitTests.AppServices.System.Fixtures.Seeding;

/// <summary>
/// Seeder for claim bonus command handler test data.
/// </summary>
public static class ClaimBonusCommandHandlerSeeder
{
    /// <summary>
    /// Seeds the database with test data for claim bonus command handler tests.
    /// </summary>
    /// <param name="context">Database context to seed.</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var now = DateTimeOffset.Parse("2026-07-01T14:37:08.645", CultureInfo.InvariantCulture);

        // Seed users
        context.Users.AddRange(
            new UserEntity
            {
                Id = 1,
                ExternalUserId = 1001,
                SystemType = 1,
                FirstName = "God user",
                LastName = "God user",
                UserName = "God user",
                CreatedAt = now,
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
                CreatedAt = now,
                Status = 1
            },
            new UserEntity
            {
                Id = 3,
                ExternalUserId = 1003,
                SystemType = 1,
                FirstName = "John",
                LastName = "Doe",
                UserName = "John.Doe",
                CreatedAt = now,
                Status = 1
            },
            new UserEntity
            {
                Id = 4,
                ExternalUserId = 1004,
                SystemType = 1,
                FirstName = "Billy",
                LastName = "Dragon",
                UserName = "Billy.Dragon",
                CreatedAt = now,
                Status = 1
            });

        // Save users to get their IDs
        context.SaveChanges();

        // Seed accounts
        context.Accounts.AddRange(
            new AccountEntity
            {
                Id = 1,
                UserId = 1,
                Balance = 10000000.00m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 2,
                UserId = 2,
                Balance = 1500.50m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status =AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 3,
                UserId = 3,
                Balance = 3200.00m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 4,
                UserId = 4,
                Balance = 3200.00m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            });

        // Seed bonus stats for users
        context.UserBonusStats.AddRange(
            // User 1002 (Bob) - already claimed daily bonus, not available yet
            new UserBonusStatsEntity
            {
                UserId = 2,
                IsStartBonusClaimed = true,
                CurrentDailyStreak = 2,
                TotalDailyBonusesClaimed = 200.00m,
                TotalStreakBonusesClaimed = 0,
                LastDailyClaimedAt = now.AddHours(-12),
                NextDailyAvailableAt = now.AddHours(12),
                TotalStartBonusesClaimed = 1000.00m
            },
            // User 1003 (John) - already claimed daily bonus, not available yet
            new UserBonusStatsEntity
            {
                UserId = 3,
                IsStartBonusClaimed = true,
                CurrentDailyStreak = 2,
                TotalDailyBonusesClaimed = 200.00m,
                TotalStreakBonusesClaimed = 0,
                LastDailyClaimedAt = now.AddHours(-12),
                NextDailyAvailableAt = now.AddHours(12),
                TotalStartBonusesClaimed = 1000.00m
            },
            // User 1004 (Billy) - streak = 6, ready for weekly streak bonus
            new UserBonusStatsEntity
            {
                UserId = 4,
                IsStartBonusClaimed = true,
                CurrentDailyStreak = 6,
                TotalDailyBonusesClaimed = 600.00m,
                TotalStreakBonusesClaimed = 0,
                LastDailyClaimedAt = now.AddHours(-24),
                NextDailyAvailableAt = now,
                TotalStartBonusesClaimed = 1000.00m
            });
            
        context.SaveChanges();

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

        context.SaveChanges();
    }
}
