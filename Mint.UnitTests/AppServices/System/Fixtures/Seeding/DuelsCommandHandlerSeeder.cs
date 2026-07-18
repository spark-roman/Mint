using Mint.Database;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Initializers;
using Mint.Database.Entities.UserInteractive.UserCategories;
using Mint.Database.Entities.Users;

namespace Mint.UnitTests.AppServices.System.Fixtures.Seeding;

/// <summary>
/// Seeder for duels command handler test data.
/// </summary>
public static class DuelsCommandHandlerSeeder
{
    /// <summary>
    /// Seeds the database with test data for duels command handler tests.
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
            if (button.Id == 9)
            {
                button.OrderNum = 3;
            }

            context.Buttons.Add(button);
        }

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
            },
            new UserEntity
            {
                Id = 3,
                ExternalUserId = 1003,
                SystemType = 1,
                FirstName = "Charlie",
                LastName = "Brown",
                UserName = "charlie_b",
                CreatedAt = DateTimeOffset.UtcNow,
                Status = 1
            });

        context.SaveChanges();

        // Seed active categories for duels
        context.UserCategories.AddRange(
            new CategoryEntity
            {
                Name = "Криптовалюта",
                Description = "Все о криптовалютах и блокчейне",
                Code = "crypto",
                IsActiveForAI = true,
                SearchKeywords = "Bitcoin, Ethereum, TON, аирдропы"
            },
            new CategoryEntity
            {
                Name = "Технологии",
                Description = "IT, гаджеты и софт",
                Code = "tech",
                IsActiveForAI = true,
                SearchKeywords = "AI, нейросети, iOS, Android"
            },
            new CategoryEntity
            {
                Name = "Спорт",
                Description = "Футбол, баскетбол, UFC и другое",
                Code = "sports",
                IsActiveForAI = true,
                SearchKeywords = "Чемпионат мира, UFC, НБА"
            });

        context.SaveChanges();
    }
}
