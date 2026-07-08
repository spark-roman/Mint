using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Database;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Bonuses;
using Mint.Database.Entities.Users;

namespace Mint.UnitTests.AppServices.UsersInteractive.Seeding;

/// <summary>
/// Seeder for bonus calculation handler test data.
/// </summary>
public static class BonusCalculationHandlerSeeder
{
    /// <summary>
    /// Seeds the database with test data for bonus calculation handler tests.
    /// </summary>
    /// <param name="context">Database context to seed.</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var now = DateTimeOffset.UtcNow;

        context.Users.AddRange(
            new UserEntity
            {
                Id = 1,
                ExternalUserId = 1001,
                SystemType = 1,
                FirstName = "Alice",
                LastName = "Smith",
                UserName = "alice.smith",
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
                FirstName = "Charlie",
                LastName = "Brown",
                UserName = "charlie.brown",
                CreatedAt = now,
                Status = 1
            });

        context.Accounts.AddRange(
            new AccountEntity
            {
                Id = 1,
                UserId = 0,
                Balance = 10000000.00m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = Mint.Common.Contracts.Ledger.Accounts.AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 2,
                UserId = 1,
                Balance = 1500.50m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = Mint.Common.Contracts.Ledger.Accounts.AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 3,
                UserId = 2,
                Balance = 3200.00m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = Mint.Common.Contracts.Ledger.Accounts.AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 4,
                UserId = 3,
                Balance = 750.25m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = Mint.Common.Contracts.Ledger.Accounts.AccountStatus.Active
            });

        context.UserBonusStats.AddRange(
            new UserBonusStatsEntity
            {
                Id = 1,
                UserId = 1,
                IsStartBonusClaimed = true,
                CurrentDailyStreak = 3,
                LastDailyClaimedAt = now.AddDays(-1),
                NextDailyAvailableAt = now.AddHours(12),
                TotalStartBonusesClaimed = 1000.00m,
                TotalDailyBonusesClaimed = 300.00m,
                TotalStreakBonusesClaimed = 0,
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
                TotalStartBonusesClaimed = 0,
                TotalDailyBonusesClaimed = 0,
                TotalStreakBonusesClaimed = 0,
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
