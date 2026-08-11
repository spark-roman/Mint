using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.Users;

namespace Mint.Database.Seeding;

/// <summary>
/// Seeder for regular (non-system) test users and accounts.
/// Does NOT create system user (Id=1) or system account (Id=1).
/// </summary>
public static class PayoutUsersSeeder
{
    /// <summary>
    /// System user ID.
    /// </summary>
    public const long SystemUserId = 1;

    /// <summary>
    /// System account ID.
    /// </summary>
    public const long SystemAccountId = 1;

    /// <summary>
    /// System account balance - huge amount for payout tests.
    /// </summary>
    public const decimal SystemAccountBalance = 999_999_999.99m;

    /// <summary>
    /// Seed regular test users and accounts into database context.
    /// </summary>
    /// <param name="context">Database context to seed.</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Users.Add(new UserEntity
        {
            Id = SystemUserId,
            ExternalUserId = 1001,
            SystemType = 1,
            FirstName = "System",
            LastName = "Account",
            UserName = "system.account",
            CreatedAt = DateTimeOffset.Now,
            Status = 1
        });

        context.Accounts.Add(new AccountEntity
        {
            Id = SystemAccountId,
            UserId = SystemUserId,
            Balance = SystemAccountBalance,
            CreatedAt = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero),
            LastTransactionDate = new DateTimeOffset(2024, 6, 20, 14, 45, 0, TimeSpan.Zero),
            Status = AccountStatus.Active
        });

        context.Users.AddRange(
            new UserEntity
            {
                Id = 2,
                ExternalUserId = 1002,
                SystemType = 1,
                FirstName = "Alice",
                LastName = "Smith",
                UserName = "alice.smith",
                CreatedAt = DateTimeOffset.Now,
                Status = 1
            },
            new UserEntity
            {
                Id = 3,
                ExternalUserId = 1003,
                SystemType = 2,
                FirstName = "Bob",
                LastName = "Johnson",
                UserName = "bob.johnson",
                CreatedAt = DateTimeOffset.Now,
                Status = 1
            });

        context.Accounts.AddRange(
            new AccountEntity
            {
                Id = 2,
                UserId = 2,
                Balance = 3200.00m,
                CreatedAt = new DateTimeOffset(2024, 2, 10, 9, 0, 0, TimeSpan.Zero),
                LastTransactionDate = new DateTimeOffset(2024, 7, 5, 16, 30, 0, TimeSpan.Zero),
                Status = AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 3,
                UserId = 3,
                Balance = 750.25m,
                CreatedAt = new DateTimeOffset(2024, 3, 5, 11, 15, 0, TimeSpan.Zero),
                LastTransactionDate = new DateTimeOffset(2024, 5, 18, 12, 0, 0, TimeSpan.Zero),
                Status = AccountStatus.Active
            });

        context.Duels.AddRange(new DuelEntity
        {
            Id = 100500,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "ИИ заменит программистов к 2025?",
            Description = "Серьезный вопрос о будущем профессии",
            ExpiresAt = DateTimeOffset.MaxValue,
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionEntity { Id = 100501, OptionText = "Да", OptionCode = "yes" },
                new DuelOptionEntity { Id = 100502, OptionText = "Нет", OptionCode = "no" }
            ]
        });
    }
}
