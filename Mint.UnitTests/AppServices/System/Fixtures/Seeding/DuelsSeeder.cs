using System.Collections.ObjectModel;
using System.Text.Json;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.Database;
using Mint.Database.Entities.System.Initializers;
using Mint.Database.Entities.UserInteractive.UserCategories.Initializers;

namespace Mint.UnitTests.AppServices.System.Fixtures.Seeding;

/// <summary>
/// Seeder for duel generation test data using EF Core entities.
/// </summary>
public static class DuelsSeeder
{
    /// <summary>
    /// Seeds the database with AI prompts and categories using EF Core entities.
    /// </summary>
    /// <param name="context">Database context to seed.</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var promptInitializer = new PromptInitializer();
        var categoryInitializer = new CategoryInitializer();

        var prompts = promptInitializer.Get();
        var categories = categoryInitializer.Get();

        foreach (var prompt in prompts)
        {
            context.AiPrompts.Add(prompt);
        }

        foreach (var category in categories)
        {
            context.UserCategories.Add(category);
        }

        context.SaveChanges();
    }

    /// <summary>
    /// Creates a valid DeepSeek API response JSON string.
    /// </summary>
    /// <param name="duels">Collection of duel generation DTOs.</param>
    /// <returns>JSON string representing the API response.</returns>
    public static string CreateValidApiResponse(Collection<DuelGenerationDto> duels)
    {
        var choices = new DeepSeekResponse
        {
            Choices =
            [
                new() {
                    Message = new DeepSeekMessageDto
                    {
                        Content = JsonSerializer.Serialize(duels)
                    }
                }
            ]
        };

        return JsonSerializer.Serialize(choices);
    }
}
