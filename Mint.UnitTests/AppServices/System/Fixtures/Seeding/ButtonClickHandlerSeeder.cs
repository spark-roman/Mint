using Mint.Database;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Initializers;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.UserCategories;
using Mint.Database.Entities.Users;
using Mint.Common.Contracts.UserInteractive;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Common.Contracts.Ledger.Accounts;

namespace Mint.UnitTests.AppServices.System.Fixtures.Seeding;

/// <summary>
/// Seeder for button click handler test data.
/// </summary>
public static class ButtonClickHandlerSeeder
{
    /// <summary>
    /// Seeds the database with test data for button click handler tests.
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

        var now = DateTimeOffset.UtcNow;

        // Seed test users for session tracking
        context.Users.AddRange(
            new UserEntity
            {
                Id = 1,
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
                Id = 2,
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
                Id = 3,
                ExternalUserId = 1003,
                SystemType = 1,
                FirstName = "Charlie",
                LastName = "Brown",
                UserName = "charlie_b",
                CreatedAt = now,
                Status = 1
            });

        context.SaveChanges();

        context.Accounts.AddRange(
            new AccountEntity
            {
                Id = 1,
                UserId = 1,
                Balance = 10000000.00m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 2,
                UserId = 2,
                Balance = 1500.50m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status =AccountStatus.Active
            },
            new AccountEntity
            {
                Id = 3,
                UserId = 3,
                Balance = 3200.00m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            });

        context.SaveChanges();

        // Seed active categories for duels
        context.UserCategories.AddRange(
            new CategoryEntity
            {
                Id = 1,
                Name = "Криптовалюта",
                Description = "Все о криптовалютах и блокчейне",
                Code = "crypto",
                IsActiveForAI = true,
                SearchKeywords = "Bitcoin, Ethereum, TON, аирдропы"
            },
            new CategoryEntity
            {
                Id = 2,
                Name = "Технологии",
                Description = "IT, гаджеты и софт",
                Code = "tech",
                IsActiveForAI = true,
                SearchKeywords = "AI, нейросети, iOS, Android"
            },
            new CategoryEntity
            {
                Id = 3,
                Name = "Спорт",
                Description = "Футбол, баскетбол, UFC и другое",
                Code = "sports",
                IsActiveForAI = true,
                SearchKeywords = "Чемпионат мира, UFC, НБА"
            });

        context.SaveChanges();

        // Seed duels with future expiration dates
        var duel1 = new DuelEntity
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $150K в 2025?",
            Description = "Достижит ли Bitcoin цены в 150 тысяч долларов до конца 2025 года?",
            ExpiresAt = now.AddHours(48),
            Options = new List<DuelOptionEntity>
            {
                new() { Id = 1, OptionText = "Да, конечно!", OptionCode = "yes" },
                new() { Id = 2, OptionText = "Нет, не думаю", OptionCode = "no" }
            }
        };

        var duel2 = new DuelEntity
        {
            Id = 2,
            CategoryId = 2,
            DuelType = DuelType.OpinionMatch,
            Question = "Заменят ли нейросети программистов?",
            Description = "Станут ли нейросети полноценной заменой разработчиков к 2030 году?",
            ExpiresAt = now.AddHours(72),
            Options = new List<DuelOptionEntity>
            {
                new() { Id = 3, OptionText = "Да, это неизбежно", OptionCode = "yes" },
                new() { Id = 4, OptionText = "Нет, люди нужны", OptionCode = "no" }
            }
        };

        var duel3 = new DuelEntity
        {
            Id = 3,
            CategoryId = 3,
            DuelType = DuelType.OpinionMatch,
            Question = "Россия выиграет ЧМ-2026?",
            Description = "Сможет ли сборная России выйти в финал Чемпионата Мира по футболу 2026?",
            ExpiresAt = now.AddHours(96),
            Options = new List<DuelOptionEntity>
            {
                new() { Id = 5, OptionText = "Обязательно!", OptionCode = "yes" },
                new() { Id = 6, OptionText = "Сложно сказать", OptionCode = "maybe" }
            }
        };

        context.Duels.AddRange(duel1, duel2, duel3);
        context.SaveChanges();
    }
}
