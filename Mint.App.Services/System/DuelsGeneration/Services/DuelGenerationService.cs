using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.App.Services.System.DuelsGeneration.GigaChat;
using Mint.App.Services.System.DuelsGeneration.Validators;
using Mint.App.Services.System.DuelsGeneration.Processors;
using Mint.App.Services.System.DuelsGeneration.Prompts;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.System.Prompts.Dto;
using Mint.Database.Entities.System.Prompts.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Services;

/// <inheritdoc />
public class DuelGenerationService : IDuelGenerationService, IDisposable
{
    private readonly IPromptsGenerator _promptsGenerator;

    private readonly IDtoMapper<DuelGenerationDto, DuelCreateDto> _duelMapper;

    private readonly IAIResponseProcessor _aiResponseProcessor;

    private readonly HttpClient _httpClient;

    private readonly DeepSeekSettings _settings;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly IGigaChatService _gigaChatService;

    private readonly GigaChatSettings _gigaChatSettings;

    private readonly DuelGenerationValidator _duelGenerationValidator;

    private readonly TimeProvider _timeProvider;

    private readonly ILogger<DuelGenerationService> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="promptsGenerator">Prompt generator</param>
    /// <param name="duelMapper">Duel mapper</param>
    /// <param name="aiResponseProcessor">AI response processor</param>
    /// <param name="settings">DeepSeek settings</param>
    /// <param name="httpClient">HTTP client for API calls</param>
    /// <param name="serviceScopeFactory">Service scope factory</param>
    /// <param name="gigaChatService">GigaChat service</param>
    /// <param name="gigaChatSettings">GigaChat settings</param>
    /// <param name="timeProvider">Time provider</param>
    /// <param name="logger">Logger</param>
    public DuelGenerationService(
        IPromptsGenerator promptsGenerator,
        IDtoMapper<DuelGenerationDto, DuelCreateDto> duelMapper,
        IAIResponseProcessor aiResponseProcessor,
        IOptions<DeepSeekSettings> settings,
        HttpClient httpClient,
        IServiceScopeFactory serviceScopeFactory,
        IGigaChatService gigaChatService,
        IOptions<GigaChatSettings> gigaChatSettings,
        TimeProvider timeProvider,
        ILogger<DuelGenerationService> logger)
    {
        _promptsGenerator = promptsGenerator ?? throw new ArgumentNullException(nameof(promptsGenerator));
        _duelMapper = duelMapper ?? throw new ArgumentNullException(nameof(duelMapper));
        _aiResponseProcessor = aiResponseProcessor ?? throw new ArgumentNullException(nameof(aiResponseProcessor));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = (settings ?? throw new ArgumentNullException(nameof(settings))).Value;
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _gigaChatService = gigaChatService ?? throw new ArgumentNullException(nameof(gigaChatService));
        _gigaChatSettings = (gigaChatSettings ?? throw new ArgumentNullException(nameof(gigaChatSettings))).Value;
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        _duelGenerationValidator = new();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<List<DuelCreateDto>> GenerateDuelsAsync(int promptId, int count, CancellationToken cancellationToken)
    {
        var scope = _serviceScopeFactory.CreateScope();
        var aiPromptRepository = scope.ServiceProvider.GetRequiredService<IAiPromptRepository>();
        var activePrompt = await aiPromptRepository.GetPromptAsync(promptId, cancellationToken)
            ?? throw new InvalidOperationException($"Prompt with Id: {promptId} not found");

        var activeCategories = activePrompt.Categories
            .Where(c => c.IsActiveForAI)
            .ToList();

        if (activeCategories.Count == 0)
        {
            throw new ArgumentException("No active categories found for this prompt");
        }
            
        var allDuels = new List<DuelCreateDto>();

        foreach (var category in activeCategories)
        {
            var categoryDuels = await GenerateDuelsForCategoryAsync(
                activePrompt, 
                category, 
                Math.Min(count, activePrompt.MaxDuelsPerRun)
            );

            allDuels.AddRange(categoryDuels);
        }

        return allDuels;
    }

    /// <inheritdoc />
    public async Task<List<DuelCreateDto>> GenerateDuelsForAllActiveCategoriesAsync(int maxPerCategory, CancellationToken cancellationToken)
    {
        var scope = _serviceScopeFactory.CreateScope();
        var aiPromptRepository = scope.ServiceProvider.GetRequiredService<IAiPromptRepository>();
        var activePrompts = await aiPromptRepository.GetPromptsAsync(cancellationToken) ?? throw new InvalidOperationException("Prompt not found");

        var allDuels = new List<DuelCreateDto>();

        foreach (var prompt in activePrompts)
        {
            foreach (var category in prompt.Categories)
            {
                var duels = await GenerateDuelsForCategoryAsync(
                    prompt,
                    category,
                    Math.Min(maxPerCategory, prompt.MaxDuelsPerRun)
                );
                allDuels.AddRange(duels);
            }
        }

        return allDuels;
    }

    private async Task<List<DuelCreateDto>> GenerateDuelsForCategoryAsync(AiPromptDto prompt, CategoryDto category, int count)
    {
        var now = _timeProvider.GetLocalNow();
        var systemPrompt = _promptsGenerator.GetSystemPrompt(prompt, now);
        var userPrompt = _promptsGenerator.GetUserPrompt(prompt, category, count, now);

        var request = new
        {
            model = "deepseek-chat",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            temperature = prompt.Temperature,
            response_format = new { type = "json_object" }
        };

        var requestJson = JsonSerializer.Serialize(request);
        using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(_settings.DeepSeekApiUrl, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"DeepSeek API error: {error}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DeepSeekResponse>(responseContent);
        
        if (result?.Choices == null || result.Choices.Count == 0)
        {
            throw new InvalidOperationException("No choices returned from DeepSeek");
        }
        
        var responseInnerContent = result.Choices.First().Message.Content;
        
        var expiresAt = new DateTimeOffset(now.Date.AddDays(_gigaChatSettings.DuelExpirationDays).Add(_gigaChatSettings.ExpirationTime), now.Offset);
        return await _aiResponseProcessor.Process(responseInnerContent, category.Id, expiresAt);
    }

    /// <inheritdoc />
    public async Task<List<DuelCreateDto>> GenerateDuelsWithGigaChatAsync(int promptId, int count, CancellationToken cancellationToken)
    {
        var scope = _serviceScopeFactory.CreateScope();
        var aiPromptRepository = scope.ServiceProvider.GetRequiredService<IAiPromptRepository>();
        var activePrompt = await aiPromptRepository.GetPromptAsync(promptId, cancellationToken)
            ?? throw new InvalidOperationException($"Prompt with Id: {promptId} not found");

        var allDuels = new List<DuelCreateDto>();

        foreach (var category in activePrompt.Categories)
        {
            var categoryDuels = await GenerateDuelsForCategoryWithGigaChatAsync(
                activePrompt,
                category,
                count,
                cancellationToken);

            allDuels.AddRange(categoryDuels);

            break;
        }

        return allDuels;
    }

    /// <inheritdoc />
    public async Task<List<DuelCreateDto>> GenerateDuelsWithGigaChatFromNewsAsync(
        int count,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken)
    {
        var scope = _serviceScopeFactory.CreateScope();
        var newsRepository = scope.ServiceProvider.GetRequiredService<Mint.Database.Entities.News.Repositories.INewsRepository>();

        var unprocessedNews = await newsRepository.GetUnprocessedAsync(50, null, cancellationToken);

        if (unprocessedNews.Count == 0)
        {
            _logger.LogInformation("No unprocessed news found for duel generation");
            return [];
        }

        var newsText = string.Join("\n---\n", unprocessedNews.Select(n => $"Title: {n.Title}\nDescription: {n.Description}"));

        var userPrompt = GeneratePromptFromNews(newsText, count);

        var gigaChatDuels = await _gigaChatService.GenerateDuelsAsync(userPrompt, count, cancellationToken);

        var validationResult = _duelGenerationValidator.Validate(new Collection<DuelGenerationDto>(gigaChatDuels));

        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"Duels generation validation failed: {validationResult.Message}");
        }

        var duelsCreateDtos = gigaChatDuels
            .Select(d => _duelMapper.Map(d, 0, expiresAt))
            .ToList();

        await newsRepository.MarkAsProcessedAsync(
            new Collection<long>(unprocessedNews.Select(n => n.Id).ToList()),
            cancellationToken);

        return duelsCreateDtos;
    }

    private async Task<List<DuelCreateDto>> GenerateDuelsForCategoryWithGigaChatAsync(
        AiPromptDto prompt,
        CategoryDto category,
        int count,
        CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetLocalNow();
        var systemPrompt = _promptsGenerator.GetSystemPrompt(prompt, now);
        var userPrompt = _promptsGenerator.GetUserPrompt(prompt, category, count, now);
        var fullPrompt = $"{systemPrompt}\n\n{userPrompt}";
        var gigaChatDuels = await _gigaChatService.GenerateDuelsAsync(fullPrompt, count, cancellationToken);
        var expiresAt = new DateTimeOffset(now.Date.AddDays(_gigaChatSettings.DuelExpirationDays).Add(_gigaChatSettings.ExpirationTime), now.Offset);
        var validationResult = _duelGenerationValidator.Validate(new Collection<DuelGenerationDto>(gigaChatDuels));

        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"Duels generation validation failed: {validationResult.Message}");
        }

        var duelsCreateDtos = gigaChatDuels
            .Select(d => _duelMapper.Map(d, category.Id, expiresAt))
            .ToList();

        _logger.LogInformation(
            "Generated {Count} duels for category '{CategoryName}' using GigaChat",
            duelsCreateDtos.Count,
            category.Name);

        return duelsCreateDtos;
    }

    private static string GeneratePromptFromNews(string newsText, int count)
    {
        return $"""
            You are an AI assistant that creates engaging opinion duels from news articles.
            Create {count} duels based on the following news articles.

            NEWS ARTICLES:
            {newsText}

            For each duel, provide:
            - Title: A catchy title with emoji
            - Description: A brief description of the conflict
            - Options: 2-4 answer variants (e.g., "For", "Against", "Neutral", "Humorous")

            Return the result as a JSON array of objects with fields: Title, Description, Options.
            """;
    }

    private bool _disposed;

    /// <inheritdoc />
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _httpClient?.Dispose();
        }

        _disposed = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
