using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Database;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Initializers;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Transactions;
using Mint.Database.Entities.UserInteractive.Bonuses;
using Mint.Database.Entities.UserInteractive.Stats;
using Mint.Database.Entities.UserInteractive.Stats.Initializers;
using Mint.Database.Entities.Users;

namespace Mint.UnitTests.AppServices.UsersInteractive.Seeding;

/// <summary>
/// Seeder for user profiles handler test data using EF Core entities.
/// </summary>
public static class UserProfilesHandlerSeeder
{
    /// <summary>
    /// Seeds the database with test data for user profiles handler tests.
    /// </summary>
    /// <param name="context">Database context to seed.</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var rankInitializer = new RankConfigInitializer();
        var rankConfigs = rankInitializer.Get();

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
            },
            new UserEntity
            {
                Id = 3,
                ExternalUserId = 1003,
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
                Id = 1,
                UserId = 0,
                Balance = 1000000.00m,
                CreatedAt = DateTimeOffset.UtcNow,
                LastTransactionDate = DateTimeOffset.UtcNow,
                Status = AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 2,
                UserId = 1,
                Balance = 1500.50m,
                CreatedAt = DateTimeOffset.UtcNow,
                LastTransactionDate = DateTimeOffset.UtcNow,
                Status = AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 3,
                UserId = 2,
                Balance = 3200.00m,
                CreatedAt = DateTimeOffset.UtcNow,
                LastTransactionDate = DateTimeOffset.UtcNow,
                Status = AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 4,
                UserId = 3,
                Balance = 750.25m,
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
                UpdatedAt = DateTimeOffset.UtcNow
            },
            new UserStatsEntity
            {
                Id = 2,
                UserId = 2,
                RankPoints = 75,
                TotalWins = 5,
                TotalLosses = 8,
                ReferralCount = 0,
                UpdatedAt = DateTimeOffset.UtcNow
            },
            new UserStatsEntity
            {
                Id = 3,
                UserId = 3,
                RankPoints = 500,
                TotalWins = 20,
                TotalLosses = 10,
                ReferralCount = 5,
                UpdatedAt = DateTimeOffset.UtcNow
            });

        var now = DateTimeOffset.UtcNow;

        context.UserBonusStats.AddRange(
            new UserBonusStatsEntity
            {
                Id = 1,
                UserId = 1,
                IsStartBonusClaimed = true,
                CurrentDailyStreak = 3,
                LastDailyClaimedAt = now.AddDays(-1),
                NextDailyAvailableAt = now.AddHours(12),
                TotalReferralBonusesClaimed = 1,
                LastRankBonusClaimedAt = now.AddDays(-3)
            },
            new UserBonusStatsEntity
            {
                Id = 2,
                UserId = 2,
                IsStartBonusClaimed = false,
                CurrentDailyStreak = 0,
                LastDailyClaimedAt = null,
                NextDailyAvailableAt = now.AddHours(-1),
                TotalReferralBonusesClaimed = 0,
                LastRankBonusClaimedAt = null
            });

        context.BonusTypes.AddRange(
            new BonusTypeEntity
            {
                Id = 1,
                Code = "start",
                Name = "Start Bonus",
                Description = "Start bonus for new users",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new BonusTypeEntity
            {
                Id = 2,
                Code = "daily",
                Name = "Daily Bonus",
                Description = "Daily login bonus",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new BonusTypeEntity
            {
                Id = 3,
                Code = "streak",
                Name = "Streak Bonus",
                Description = "Streak bonus for consecutive logins",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            });

        context.SaveChanges();
    }
}
