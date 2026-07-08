using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Database;
using Mint.Database.Entities.Bot.Commands;
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

        var now = DateTimeOffset.UtcNow;

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
                Status = Mint.Common.Contracts.Ledger.Accounts.AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 2,
                UserId = 2,
                Balance = 1500.50m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = Mint.Common.Contracts.Ledger.Accounts.AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 3,
                UserId = 3,
                Balance = 3200.00m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = Mint.Common.Contracts.Ledger.Accounts.AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 4,
                UserId = 4,
                Balance = 3200.00m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = Mint.Common.Contracts.Ledger.Accounts.AccountStatus.Active
            });

        // Seed bonus stats for user 1003 with streak = 6 (ready for weekly streak bonus)
        context.UserBonusStats.AddRange(
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
    }
}
