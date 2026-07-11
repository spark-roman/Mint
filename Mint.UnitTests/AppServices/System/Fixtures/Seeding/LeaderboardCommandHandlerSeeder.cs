using Mint.Database;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Initializers;
using Mint.Database.Entities.UserInteractive.Stats;
using Mint.Database.Entities.Users;

namespace Mint.UnitTests.AppServices.System.Fixtures.Seeding;

/// <summary>
/// Seeder for leaderboard command handler test data.
/// </summary>
public static class LeaderboardCommandHandlerSeeder
{
    /// <summary>
    /// Seeds the database with test data for leaderboard command handler tests.
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

        // Seed rank configs
        context.RankConfigs.AddRange(
            new RankConfigEntity
            {
                Id = 1,
                Code = "newbie",
                Name = "Новичок",
                Emoji = "🌱",
                MinPoints = 0
            },
            new RankConfigEntity
            {
                Id = 2,
                Code = "analyst",
                Name = "Аналитик",
                Emoji = "📊",
                MinPoints = 500
            },
            new RankConfigEntity
            {
                Id = 3,
                Code = "oracle",
                Name = "Оракул",
                Emoji = "👁️",
                MinPoints = 1000
            });

        // Seed users with different rank points for leaderboard
        context.Users.AddRange(
            new UserEntity
            {
                Id = 1,
                ExternalUserId = 1001,
                SystemType = 1,
                FirstName = "Alice",
                LastName = "Johnson",
                UserName = "alice_j",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 2,
                ExternalUserId = 1002,
                SystemType = 1,
                FirstName = "Bob",
                LastName = "Smith",
                UserName = "bob_s",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 3,
                ExternalUserId = 1003,
                SystemType = 1,
                FirstName = "Charlie",
                LastName = "Brown",
                UserName = "charlie_b",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 4,
                ExternalUserId = 1004,
                SystemType = 1,
                FirstName = "Diana",
                LastName = "Prince",
                UserName = "diana_p",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 5,
                ExternalUserId = 1005,
                SystemType = 1,
                FirstName = "Eve",
                LastName = "Davis",
                UserName = "eve_d",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            });

        context.SaveChanges();

        // Seed user stats with different rank points
        // User 1001 (Alice) has highest points - rank #1
        // User 1003 (Charlie) has second highest - rank #2
        // User 1002 (Bob) has third - rank #3
        // User 1004 (Diana) has lowest - rank #4
        // User 1005 (Eve) has no stats - not in leaderboard
        context.UserStats.AddRange(
            new UserStatsEntity
            {
                Id = 1,
                UserId = 1,
                RankPoints = 1500,
                TotalWins = 30,
                TotalLosses = 5,
                ReferralCount = 3,
                UpdatedAt = DateTimeOffset.UtcNow
            },
            new UserStatsEntity
            {
                Id = 2,
                UserId = 2,
                RankPoints = 800,
                TotalWins = 15,
                TotalLosses = 10,
                ReferralCount = 1,
                UpdatedAt = DateTimeOffset.UtcNow
            },
            new UserStatsEntity
            {
                Id = 3,
                UserId = 3,
                RankPoints = 1200,
                TotalWins = 25,
                TotalLosses = 8,
                ReferralCount = 2,
                UpdatedAt = DateTimeOffset.UtcNow
            },
            new UserStatsEntity
            {
                Id = 4,
                UserId = 4,
                RankPoints = 300,
                TotalWins = 5,
                TotalLosses = 10,
                ReferralCount = 0,
                UpdatedAt = DateTimeOffset.UtcNow
            },
            new UserStatsEntity
            {
                Id = 5,
                UserId = 5,
                RankPoints = 1300,
                TotalWins = 5,
                TotalLosses = 10,
                ReferralCount = 0,
                UpdatedAt = DateTimeOffset.UtcNow
            });

        context.SaveChanges();
    }
}
