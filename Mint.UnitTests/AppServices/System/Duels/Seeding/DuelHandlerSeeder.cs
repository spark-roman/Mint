using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.UserCategories;
using Mint.Database.Entities.Users;

namespace Mint.UnitTests.AppServices.System.Duels.Seeding;

/// <summary>
/// Seeder for duel handler test data.
/// </summary>
public static class DuelHandlerSeeder
{
    /// <summary>
    /// Seeds the database with test data for duel handler tests.
    /// </summary>
    /// <param name="context">Database context to seed.</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var now = DateTimeOffset.UtcNow;

        context.Users.Add(
            new UserEntity
            {
                Id = 1,
                ExternalUserId = 0,
                SystemType = 1,
                FirstName = "System",
                LastName = "Account",
                UserName = "system",
                CreatedAt = now,
                Status = 1
            });

        context.Users.AddRange(
            new UserEntity
            {
                Id = 2,
                ExternalUserId = 1001,
                SystemType = 1,
                FirstName = "Alice",
                LastName = "Johnson",
                UserName = "alice_j",
                CreatedAt = now,
                Status = 1
            },
            new UserEntity
            {
                Id = 3,
                ExternalUserId = 1002,
                SystemType = 1,
                FirstName = "Bob",
                LastName = "Smith",
                UserName = "bob_s",
                CreatedAt = now,
                Status = 1
            },
            new UserEntity
            {
                Id = 4,
                ExternalUserId = 1003,
                SystemType = 1,
                FirstName = "Charlie",
                LastName = "Brown",
                UserName = "charlie_b",
                CreatedAt = now,
                Status = 1
            });

        context.SaveChanges();
        
        context.Accounts.Add(
            new AccountEntity
            {
                Id = 1,
                UserId = 1,
                Balance = 1000000000m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            });

        context.Accounts.AddRange(
            new AccountEntity
            {
                Id = 2,
                UserId = 2,
                Balance = 10000m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 3,
                UserId = 3,
                Balance = 10000m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 4,
                UserId = 4,
                Balance = 10000m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            });

        context.UserCategories.AddRange(
            new CategoryEntity
            {
                Id = 3,
                Name = "Crypto",
                Description = "Cryptocurrency",
                Code = "crypto"
            },
            new CategoryEntity
            {
                Id = 4,
                Name = "Some category",
                Description = "Some test category",
                Code = "some_category"
            });

        context.SaveChanges();

        context.Duels.AddRange(
            new DuelEntity
            {
                Id = 1,
                CategoryId = 3,
                DuelType = DuelType.OpinionMatch,
                Question = "Bitcoin достигнет $100k?",
                Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
                ExpiresAt = now.AddHours(48),
                Status = DuelStatus.Active
            });

        context.SaveChanges();

        context.DuelOptions.AddRange(
            new DuelOptionEntity
            {
                Id = 1,
                DuelId = 1,
                OptionText = "Да, достигнет",
                OptionCode = "yes"
            },
            new DuelOptionEntity
            {
                Id = 2,
                DuelId = 1,
                OptionText = "Нет, не достигнет",
                OptionCode = "no"
            });

        context.SaveChanges();

        context.Duels.AddRange(
            new DuelEntity
            {
                Id = 2,
                CategoryId = 4,
                DuelType = DuelType.OpinionMatch,
                Question = "Swift лучше Kotlin?",
                Description = "Лучший язык для iOS разработки?",
                ExpiresAt = now.AddHours(24),
                Status = DuelStatus.Active
            });

        context.SaveChanges();

        context.DuelOptions.AddRange(
            new DuelOptionEntity
            {
                Id = 3,
                DuelId = 2,
                OptionText = "Swift",
                OptionCode = "swift"
            },
            new DuelOptionEntity
            {
                Id = 4,
                DuelId = 2,
                OptionText = "Kotlin",
                OptionCode = "kotlin"
            });

        context.SaveChanges();

        context.Duels.AddRange(
            new DuelEntity
            {
                Id = 3,
                CategoryId = 4,
                DuelType = DuelType.OpinionMatch,
                Question = "Ethereum 2.0?",
                Description = "Изменит ли Ethereum 2.0 рынок?",
                ExpiresAt = now.AddHours(-1),
                Status = DuelStatus.Active
            });

        context.SaveChanges();

        context.DuelOptions.AddRange(
            new DuelOptionEntity
            {
                Id = 5,
                DuelId = 3,
                OptionText = "Да",
                OptionCode = "yes"
            },
            new DuelOptionEntity
            {
                Id = 6,
                DuelId = 3,
                OptionText = "Нет",
                OptionCode = "no"
            });

        context.SaveChanges();
    }
}
