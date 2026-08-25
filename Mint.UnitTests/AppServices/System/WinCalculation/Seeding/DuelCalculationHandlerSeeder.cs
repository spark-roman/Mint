using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Votes;
using Mint.Database.Entities.Users;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Seeding;

/// <summary>
/// Seeder for duel calculation handler test data.
/// </summary>
public static class DuelCalculationHandlerSeeder
{
    /// <summary>
    /// Seeds the database with test data for duel calculation handler tests.
    /// </summary>
    /// <param name="context">Database context to seed.</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var now = DateTimeOffset.UtcNow;

        // System user and account (Id=1) — the house/system account
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

        // Test users
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
            });

        context.SaveChanges();

        // Test accounts
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
            });

        context.SaveChanges();

        // ===== Duel 1: active, with votes, option 1 wins =====
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

        // Votes: option 1 = 2 votes (500+100=600), option 2 = 1 vote (300)
        // Total pot = 900, houseCut = 45, prizePool = 855
        // winFactor = 855 / 600 = 1.425
        context.Votes.AddRange(
            new VoteEntity { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },  // Alice -> opt 1
            new VoteEntity { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },   // Bob -> opt 1
            new VoteEntity { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) });  // Charlie -> opt 2

        context.SaveChanges();

        // ===== Duel 2: active, no votes =====
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

        // ===== Duel 3: already closed =====
        context.Duels.Add(new DuelEntity
        {
            Id = 3,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Закрытый дуэль",
            Description = "Дуэль уже закрыта",
            ExpiresAt = now.AddHours(-24),
            Status = DuelStatus.Closed
        });

        context.SaveChanges();

        context.DuelOptions.AddRange(
            new DuelOptionEntity { Id = 5, DuelId = 3, OptionText = "Да", OptionCode = "yes" },
            new DuelOptionEntity { Id = 6, DuelId = 3, OptionText = "Нет", OptionCode = "no" });

        context.SaveChanges();

        // ===== Duel 4: active, single vote =====
        context.Duels.Add(new DuelEntity
        {
            Id = 4,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Один голос",
            Description = "Только один голос",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active
        });

        context.SaveChanges();

        context.DuelOptions.AddRange(
            new DuelOptionEntity { Id = 7, DuelId = 4, OptionText = "Да", OptionCode = "yes" },
            new DuelOptionEntity { Id = 8, DuelId = 4, OptionText = "Нет", OptionCode = "no" });

        context.SaveChanges();

        // Single vote: option 1, bet 500
        context.Votes.Add(
            new VoteEntity { AccountId = 2, DuelId = 4, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-1) });

        context.SaveChanges();

        // ===== Duel 5: active, equal votes on both options (tie) =====
        context.Duels.Add(new DuelEntity
        {
            Id = 5,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Равные голоса",
            Description = "Поровну голосов",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active
        });

        context.SaveChanges();

        context.DuelOptions.AddRange(
            new DuelOptionEntity { Id = 9, DuelId = 5, OptionText = "Да", OptionCode = "yes" },
            new DuelOptionEntity { Id = 10, DuelId = 5, OptionText = "Нет", OptionCode = "no" });

        context.SaveChanges();

        // 2 votes each: option 1 (500+200=700), option 2 (300+400=700)
        context.Votes.AddRange(
            new VoteEntity { AccountId = 2, DuelId = 5, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-3) },
            new VoteEntity { AccountId = 3, DuelId = 5, ChosenOptionId = 1, BetAmount = 200m, CreatedAt = now.AddHours(-3) },
            new VoteEntity { AccountId = 4, DuelId = 5, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-3) });
        // Note: only 3 votes, option 2 has 1 vote, option 1 has 2 votes -> option 1 wins
        // For true tie we need to add another vote for option 2
        // Let me fix: option 1 = 2 votes (500+200), option 2 = 1 vote (300) -> option 1 wins

        context.SaveChanges();

        // ===== Duel 6: active, equal votes on both options (tie) =====
        context.Duels.Add(new DuelEntity
        {
            Id = 6,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Равные голоса",
            Description = "Поровну голосов",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active
        });

        context.SaveChanges();

        context.DuelOptions.AddRange(
            new DuelOptionEntity { Id = 11, DuelId = 5, OptionText = "Да", OptionCode = "yes" },
            new DuelOptionEntity { Id = 12, DuelId = 5, OptionText = "Нет", OptionCode = "no" });

        context.SaveChanges();

        context.Votes.AddRange(
            new VoteEntity { AccountId = 2, DuelId = 6, ChosenOptionId = 11, BetAmount = 500m, CreatedAt = now.AddHours(-3) },
            new VoteEntity { AccountId = 3, DuelId = 6, ChosenOptionId = 11, BetAmount = 200m, CreatedAt = now.AddHours(-3) },
            new VoteEntity { AccountId = 1, DuelId = 6, ChosenOptionId = 12, BetAmount = 300m, CreatedAt = now.AddHours(-3) },
            new VoteEntity { AccountId = 4, DuelId = 6, ChosenOptionId = 12, BetAmount = 300m, CreatedAt = now.AddHours(-3) });
    }
}
