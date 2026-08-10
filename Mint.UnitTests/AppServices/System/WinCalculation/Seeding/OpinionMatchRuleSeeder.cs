using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Votes;
using Mint.Database.Entities.Users;
using Mint.Common.Contracts.Ledger.Accounts;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Seeding;

/// <summary>
/// Seeder for OpinionMatchRule test data.
/// Winning option is determined by maximum vote count (number of voters), not bet amount.
/// </summary>
public static class OpinionMatchRuleSeeder
{
    /// <summary>
    /// Seeds the database with test data for OpinionMatchRule tests.
    /// </summary>
    /// <param name="context">Database context to seed.</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var now = DateTimeOffset.UtcNow;

        // Seed system user and account (Id=1)
        context.Users.Add(new UserEntity
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

        // Seed test users
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
            },
            new UserEntity
            {
                Id = 5,
                ExternalUserId = 1004,
                SystemType = 1,
                FirstName = "Diana",
                LastName = "Prince",
                UserName = "diana_p",
                CreatedAt = now,
                Status = 1
            },
            new UserEntity
            {
                Id = 6,
                ExternalUserId = 1005,
                SystemType = 1,
                FirstName = "Eve",
                LastName = "Wilson",
                UserName = "eve_w",
                CreatedAt = now,
                Status = 1
            });

        context.SaveChanges();

        // Seed accounts
        context.Accounts.AddRange(
            new AccountEntity
            {
                Id = 1,
                UserId = 1,
                Balance = 1000000000m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            },
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
            },
            new AccountEntity
            {
                Id = 5,
                UserId = 5,
                Balance = 10000m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 6,
                UserId = 6,
                Balance = 10000m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            });

        context.SaveChanges();

        // ===== Duel 1 =====
        // Option 1: 3 votes (Alice, Charlie, Diana) -> WINNER
        // Option 2: 2 votes (Bob, Eve)
        // Note: bet amounts are intentionally varied to show that SUM doesn't matter, only COUNT
        context.Duels.Add(new DuelEntity
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active
        });

        context.SaveChanges();

        context.DuelOptions.AddRange(
            new DuelOptionEntity { Id = 1, DuelId = 1, OptionText = "Да", OptionCode = "yes" },
            new DuelOptionEntity { Id = 2, DuelId = 1, OptionText = "Нет", OptionCode = "no" });

        context.SaveChanges();

        context.Votes.AddRange(
            new VoteEntity { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) }, // Alice -> opt 1
            new VoteEntity { AccountId = 3, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }, // Bob -> opt 2
            new VoteEntity { AccountId = 4, DuelId = 1, ChosenOptionId = 1, BetAmount = 10m, CreatedAt = now.AddHours(-2) },  // Charlie -> opt 1
            new VoteEntity { AccountId = 5, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) }, // Diana -> opt 1
            new VoteEntity { AccountId = 6, DuelId = 1, ChosenOptionId = 2, BetAmount = 1000m, CreatedAt = now.AddHours(-2) }); // Eve -> opt 2
        // Vote count: option 1 = 3, option 2 = 2. Winner: option 1 (despite option 2 having more total bet: 1300 vs 610)

        context.SaveChanges();

        // ===== Duel 2 =====
        // Option 3: 1 vote (Alice)
        // Option 4: 3 votes (Diana, Eve, Charlie) -> WINNER
        context.Duels.Add(new DuelEntity
        {
            Id = 2,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Ethereum 2.0?",
            Description = "Изменит ли Ethereum 2.0 рынок?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active
        });

        context.SaveChanges();

        context.DuelOptions.AddRange(
            new DuelOptionEntity { Id = 3, DuelId = 2, OptionText = "Да", OptionCode = "yes" },
            new DuelOptionEntity { Id = 4, DuelId = 2, OptionText = "Нет", OptionCode = "no" });

        context.SaveChanges();

        context.Votes.AddRange(
            new VoteEntity { AccountId = 2, DuelId = 2, ChosenOptionId = 3, BetAmount = 9999m, CreatedAt = now.AddHours(-1) }, // Alice -> opt 3 (big bet but only 1 vote)
            new VoteEntity { AccountId = 5, DuelId = 2, ChosenOptionId = 4, BetAmount = 10m, CreatedAt = now.AddHours(-1) },     // Diana -> opt 4
            new VoteEntity { AccountId = 6, DuelId = 2, ChosenOptionId = 4, BetAmount = 10m, CreatedAt = now.AddHours(-1) },     // Eve -> opt 4
            new VoteEntity { AccountId = 4, DuelId = 2, ChosenOptionId = 4, BetAmount = 10m, CreatedAt = now.AddHours(-1) });    // Charlie -> opt 4
        // Vote count: option 3 = 1, option 4 = 3. Winner: option 4 (despite option 3 having way more total bet)

        context.SaveChanges();

        // ===== Duel 3 =====
        // Option 5: 2 votes (Alice, Bob) -> TIE
        // Option 6: 2 votes (Charlie, Diana) -> TIE
        context.Duels.Add(new DuelEntity
        {
            Id = 3,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Swift лучше Kotlin?",
            Description = "Лучший язык для iOS разработки?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active
        });

        context.SaveChanges();

        context.DuelOptions.AddRange(
            new DuelOptionEntity { Id = 5, DuelId = 3, OptionText = "Swift", OptionCode = "swift" },
            new DuelOptionEntity { Id = 6, DuelId = 3, OptionText = "Kotlin", OptionCode = "kotlin" });

        context.SaveChanges();

        context.Votes.AddRange(
            new VoteEntity { AccountId = 2, DuelId = 3, ChosenOptionId = 5, BetAmount = 100m, CreatedAt = now.AddHours(-3) }, // Alice -> opt 5
            new VoteEntity { AccountId = 3, DuelId = 3, ChosenOptionId = 5, BetAmount = 200m, CreatedAt = now.AddHours(-3) }, // Bob -> opt 5
            new VoteEntity { AccountId = 4, DuelId = 3, ChosenOptionId = 6, BetAmount = 500m, CreatedAt = now.AddHours(-3) }, // Charlie -> opt 6
            new VoteEntity { AccountId = 5, DuelId = 3, ChosenOptionId = 6, BetAmount = 50m, CreatedAt = now.AddHours(-3) });  // Diana -> opt 6
        // Vote count: option 5 = 2, option 6 = 2. TIE (both options have same vote count)

        context.SaveChanges();
    }
}
