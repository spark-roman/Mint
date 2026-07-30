using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Common.Contracts.UserInteractive;
using Mint.Database;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Transactions;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.UserCategories;
using Mint.Database.Entities.UserInteractive.Votes;
using Mint.Database.Entities.UserInteractive.Stats;
using Mint.Database.Entities.Users;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Seeding;

/// <summary>
/// Seeder for duel settlement handler test data.
/// </summary>
public static class DuelSettlementHandlerSeeder
{
    /// <summary>
    /// Seeds the database with test data for duel settlement handler tests.
    /// </summary>
    /// <param name="context">Database context to seed.</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Seed test accounts
        context.Accounts.AddRange(
            new AccountEntity
            {
                Id = 1,
                UserId = 1,
                Balance = 1000000000m,
                Status = AccountStatus.Active,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new AccountEntity
            {
                Id = 2,
                UserId = 2,
                Balance = 10000m,
                Status = AccountStatus.Active,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new AccountEntity
            {
                Id = 3,
                UserId = 3,
                Balance = 10000m,
                Status = AccountStatus.Active,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new AccountEntity
            {
                Id = 4,
                UserId = 4,
                Balance = 10000m,
                Status = AccountStatus.Active,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new AccountEntity
            {
                Id = 100,
                UserId = 5,
                Balance = 50000m,
                Status = AccountStatus.Active,
                CreatedAt = DateTimeOffset.UtcNow
            });

        context.SaveChanges();

        // Seed test users
        context.Users.AddRange(
            new UserEntity
            {
                Id = 1,
                ExternalUserId = 1001,
                SystemType = 1,
                FirstName = "System",
                LastName = "Account",
                UserName = "system",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 2,
                ExternalUserId = 1002,
                SystemType = 1,
                FirstName = "Alice",
                LastName = "Johnson",
                UserName = "alice_j",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 3,
                ExternalUserId = 1003,
                SystemType = 1,
                FirstName = "Bob",
                LastName = "Smith",
                UserName = "bob_s",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 4,
                ExternalUserId = 1004,
                SystemType = 1,
                FirstName = "Charlie",
                LastName = "Brown",
                UserName = "charlie_b",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            },
            new UserEntity
            {
                Id = 5,
                ExternalUserId = 1005,
                SystemType = 1,
                FirstName = "Diana",
                LastName = "Prince",
                UserName = "diana_p",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            });

        context.SaveChanges();

        // Seed user stats (only for accounts 2, 3, 4 — not for account 100/Diana)
        context.UserStats.AddRange(
            new UserStatsEntity
            {
                Id = 1,
                UserId = 2,
                RankPoints = 100,
                TotalWins = 5,
                TotalLosses = 2,
                ReferralCount = 0,
                UpdatedAt = DateTimeOffset.UtcNow
            },
            new UserStatsEntity
            {
                Id = 2,
                UserId = 3,
                RankPoints = 75,
                TotalWins = 3,
                TotalLosses = 3,
                ReferralCount = 0,
                UpdatedAt = DateTimeOffset.UtcNow
            },
            new UserStatsEntity
            {
                Id = 3,
                UserId = 4,
                RankPoints = 50,
                TotalWins = 2,
                TotalLosses = 4,
                ReferralCount = 0,
                UpdatedAt = DateTimeOffset.UtcNow
            });

        context.SaveChanges();

        // Seed test categories
        context.UserCategories.AddRange(
            new CategoryEntity
            {
                Id = 1,
                Name = "Криптовалюта",
                Description = "Все о криптовалютах",
                Code = "crypto",
                IsActiveForAI = true,
                SearchKeywords = "Bitcoin, Ethereum"
            },
            new CategoryEntity
            {
                Id = 2,
                Name = "Технологии",
                Description = "IT и гаджеты",
                Code = "tech",
                IsActiveForAI = true,
                SearchKeywords = "AI, нейросети"
            });

        context.SaveChanges();

        // Seed duel 1 - active, with options
        context.Duels.AddRange(
            new DuelEntity
            {
                Id = 1,
                CategoryId = 1,
                DuelType = DuelType.OpinionMatch,
                Question = "Bitcoin достигнет $100k?",
                Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(-1), // expired
                IsClosed = false
            });

        context.SaveChanges();

        // Seed duel options
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

        // Seed votes for duel 1 - option 1 wins (more bets)
        context.Votes.AddRange(
            new VoteEntity
            {
                AccountId = 2,
                DuelId = 1,
                ChosenOptionId = 1,
                BetAmount = 500m,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-2)
            },
            new VoteEntity
            {
                AccountId = 3,
                DuelId = 1,
                ChosenOptionId = 1,
                BetAmount = 300m,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-2)
            },
            new VoteEntity
            {
                AccountId = 4,
                DuelId = 1,
                ChosenOptionId = 2,
                BetAmount = 200m,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-2)
            });

        context.SaveChanges();

        // Seed duel 2 - active, with options, no votes
        context.Duels.AddRange(
            new DuelEntity
            {
                Id = 2,
                CategoryId = 1,
                DuelType = DuelType.OpinionMatch,
                Question = "Ethereum 2.0 изменит рынок?",
                Description = "Изменит ли Ethereum 2.0 рынок?",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(-1), // expired
                IsClosed = false
            });

        context.SaveChanges();

        context.DuelOptions.AddRange(
            new DuelOptionEntity
            {
                Id = 3,
                DuelId = 2,
                OptionText = "Да, изменит",
                OptionCode = "yes"
            },
            new DuelOptionEntity
            {
                Id = 4,
                DuelId = 2,
                OptionText = "Нет, не изменит",
                OptionCode = "no"
            });

        context.SaveChanges();

        // Seed duel 3 - already closed
        context.Duels.AddRange(
            new DuelEntity
            {
                Id = 3,
                CategoryId = 2,
                DuelType = DuelType.FactPrediction,
                Question = "ИИ заменит программистов?",
                Description = "Заменит ли ИИ программистов?",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(-24),
                IsClosed = true
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

        // Seed duel 4 - still active (not expired)
        context.Duels.AddRange(
            new DuelEntity
            {
                Id = 4,
                CategoryId = 2,
                DuelType = DuelType.OpinionMatch,
                Question = "Swift лучше Kotlin?",
                Description = "Лучший язык для iOS разработки?",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(24), // not expired
                IsClosed = false
            });

        context.SaveChanges();

        context.DuelOptions.AddRange(
            new DuelOptionEntity
            {
                Id = 7,
                DuelId = 4,
                OptionText = "Swift",
                OptionCode = "swift"
            },
            new DuelOptionEntity
            {
                Id = 8,
                DuelId = 4,
                OptionText = "Kotlin",
                OptionCode = "kotlin"
            });

        context.SaveChanges();

        // Seed transactions for previously settled duels
        context.Transactions.AddRange(
            new TransactionEntity
            {
                Id = 1,
                DebitAccountId = 100,
                CreditAccountId = 1,
                Amount = 600m,
                Description = "Выплата за дуэль:999",
                BonusTypeId = 1,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-3)
            });

        context.SaveChanges();
    }
}
