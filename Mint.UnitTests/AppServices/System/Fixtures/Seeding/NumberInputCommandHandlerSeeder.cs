using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Database;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Initializers;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Stats;
using Mint.Database.Entities.UserInteractive.UserCategories;
using Mint.Database.Entities.Users;
using Mint.Database.Entities.Users.Sessions;

namespace Mint.UnitTests.AppServices.System.Fixtures.Seeding;

/// <summary>
/// Seeder for number input command handler test data.
/// </summary>
public static class NumberInputCommandHandlerSeeder
{
    /// <summary>
    /// Seeds the database with test data for number input command handler tests.
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

        context.SaveChanges();

        // Seed rank configs
        context.RankConfigs.AddRange(
            new RankConfigEntity
            {
                Id = 1,
                Code = "newbie",
                Name = "Новичок",
                Emoji = "🌱",
                MinPoints = 0
            },
            new RankConfigEntity
            {
                Id = 2,
                Code = "analyst",
                Name = "Аналитик",
                Emoji = "📊",
                MinPoints = 500
            },
            new RankConfigEntity
            {
                Id = 3,
                Code = "oracle",
                Name = "Оракул",
                Emoji = "👁️",
                MinPoints = 1000
            });

        context.SaveChanges();

        // Seed user categories
        context.UserCategories.AddRange(
            new CategoryEntity
            {
                Id = 1,
                Code = "politics",
                Name = "Политика",
                Description = "Политические и социальные темы"
            },
            new CategoryEntity
            {
                Id = 2,
                Code = "science",
                Name = "Наука",
                Description = "Научные и технологические темы"
            },
            new CategoryEntity
            {
                Id = 3,
                Code = "entertainment",
                Name = "Развлечения",
                Description = "Кино, музыка и поп-культура"
            });

        context.SaveChanges();

        // Seed users
        context.Users.AddRange(
            new UserEntity
            {
                Id = 1,
                ExternalUserId = 1001,
                SystemType = 1,
                FirstName = "Alice",
                LastName = "Johnson",
                UserName = "alice_j",
                CreatedAt = DateTimeOffset.UtcNow,
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
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            });

        context.SaveChanges();

        // Seed user stats with rank points
        context.UserStats.AddRange(
            new UserStatsEntity
            {
                Id = 1,
                UserId = 1,
                RankPoints = 1500,
                TotalWins = 30,
                TotalLosses = 5,
                ReferralCount = 3,
                UpdatedAt = DateTimeOffset.UtcNow
            },
            new UserStatsEntity
            {
                Id = 2,
                UserId = 2,
                RankPoints = 800,
                TotalWins = 15,
                TotalLosses = 10,
                ReferralCount = 1,
                UpdatedAt = DateTimeOffset.UtcNow
            });

        context.SaveChanges();

        // Seed duels with options
        context.Duels.AddRange(
            new DuelEntity
            {
                Id = 1,
                CategoryId = 1,
                Question = "ИИ улучшит общество?",
                Description = "Обсудим влияние искусственного интеллекта",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
                IsClosed = false,
                Options =
                [
                    new DuelOptionEntity
                    {
                        Id = 1,
                        DuelId = 1,
                        OptionText = "Да, улучшит",
                        OptionCode = "yes"
                    },
                    new DuelOptionEntity
                    {
                        Id = 2,
                        DuelId = 1,
                        OptionText = "Нет, ухудшит",
                        OptionCode = "no"
                    }
                ]
            },
            new DuelEntity
            {
                Id = 2,
                CategoryId = 2,
                Question = "Квантовые компьютеры изменят мир?",
                Description = "Обсудим влияние квантовых вычислений",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(12),
                IsClosed = false,
                Options =
                [
                    new DuelOptionEntity
                    {
                        Id = 3,
                        DuelId = 2,
                        OptionText = "Да, радикально",
                        OptionCode = "radical"
                    },
                    new DuelOptionEntity
                    {
                        Id = 4,
                        DuelId = 2,
                        OptionText = "Нет, эволюционно",
                        OptionCode = "evolutionary"
                    }
                ]
            });

        context.SaveChanges();

        // Seed user sessions with duel data
        context.UserSessions.AddRange(
            new UserSessionEntity
            {
                Id = 1,
                UserId = 1,
                ScenarioId = 3, // duels scenario
                CurrentStepId = 9, // bet input step
                Data = "{\"duel_id\":1,\"option_id\":1,\"category\":\"politics\"}",
                StartedAt = DateTimeOffset.UtcNow,
                CompletedAt = null
            },
            new UserSessionEntity
            {
                Id = 2,
                UserId = 2,
                ScenarioId = 3, // duels scenario
                CurrentStepId = 9, // bet input step
                Data = "{\"duel_id\":2,\"option_id\":3,\"category\":\"science\"}",
                StartedAt = DateTimeOffset.UtcNow,
                CompletedAt = null
            });

        context.SaveChanges();

        var now = DateTime.UtcNow;

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
            },
            new AccountEntity
            {
                Id = 4,
                UserId = 4,
                Balance = 3200.00m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            });
    }
}
