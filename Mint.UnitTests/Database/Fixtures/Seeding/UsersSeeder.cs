using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Database.Entities.Bot.Commands.Initializers;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Stats.Initializers;
using Mint.Database.Entities.Users;

namespace Mint.Database.Seeding;

/// <summary>
/// Seeder for user entities
/// </summary>
public static class UsersSeeder
{
    /// <summary>
    /// Seed test users into database context
    /// </summary>
    /// <param name="context">Database context to seed</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Users.AddRange(
            new UserEntity
            {
                Id = 1,
                ExternalUserId = 1001,
                SystemType = 1,
                FirstName = "Alice",
                LastName = "Smith",
                UserName = "alice.smith",
                CreatedAt = DateTimeOffset.Now,
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
                CreatedAt = DateTimeOffset.Now,
                Status = 1
            },
            new UserEntity
            {
                Id = 3,
                ExternalUserId = 1003,
                SystemType = 2,
                FirstName = "Charlie",
                LastName = "Brown",
                UserName = "charlie.brown",
                CreatedAt = DateTimeOffset.Now,
                Status = 1
            });

        context.Accounts.AddRange(
            new AccountEntity
            {
                Id = 1,
                UserId = 1,
                Balance = 1500.50m,
                CreatedAt = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero),
                LastTransactionDate = new DateTimeOffset(2024, 6, 20, 14, 45, 0, TimeSpan.Zero),
                Status = AccountStatus.Active
            },
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

        context.RankConfigs.AddRange(new RankConfigInitializer().Get());
        context.StepTypes.AddRange(new StepTypeInitializer().Get());
    }
}
