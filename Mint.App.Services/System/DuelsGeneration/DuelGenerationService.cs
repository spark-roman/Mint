using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.App.Services.System.DuelsGeneration.Prompts;
using Mint.App.Services.System.DuelsGeneration.Validators;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.System.Dto;
using Mint.Database.Entities.System.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.App.Services.System.DuelsGeneration;

/// <inheritdoc />
public class DuelGenerationService : IDuelGenerationService, IDisposable
{
    private readonly IAiPromptRepository _aiPromptRepository;

    private readonly IPromptsGenerator _promptsGenerator;

    private readonly IDuelGenerationValidator _duelGenerationValidator;

    private readonly IDtoMapper<DuelGenerationDto, DuelCreateDto> _duelMapper;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _httpClient;

    private readonly DeepSeekSettings _settings;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="aiPromptRepository">Prompt repository</param>
    /// <param name="promptsGenerator">Prompt generator</param>
    /// <param name="duelGenerationValidator">Duel validation</param>
    /// <param name="duelMapper">Duel mapper</param>
    /// <param name="settings">DeepSeek settings</param>
    /// <param name="httpClient">HTTP client for API calls</param>
    public DuelGenerationService(
        IAiPromptRepository aiPromptRepository,
        IPromptsGenerator promptsGenerator,
        IDuelGenerationValidator duelGenerationValidator,
        IDtoMapper<DuelGenerationDto, DuelCreateDto> duelMapper,
        DeepSeekSettings settings,
        HttpClient httpClient)
    {
        _aiPromptRepository = aiPromptRepository ?? throw new ArgumentNullException(nameof(aiPromptRepository));
        _promptsGenerator = promptsGenerator ?? throw new ArgumentNullException(nameof(promptsGenerator));
        _duelGenerationValidator = duelGenerationValidator ?? throw new ArgumentNullException(nameof(duelGenerationValidator));
        _duelMapper = duelMapper ?? throw new ArgumentNullException(nameof(duelMapper));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    /// <inheritdoc />
    public async Task<List<DuelCreateDto>> GenerateDuelsAsync(int promptId, int count, CancellationToken cancellationToken)
    {
        var activePrompt = await _aiPromptRepository.GetPromptAsync(promptId, cancellationToken)
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
        var activePrompts = await _aiPromptRepository.GetPromptsAsync(cancellationToken) ?? throw new InvalidOperationException("Prompt not found");

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
        var systemPrompt = _promptsGenerator.GetSystemPrompt(prompt);
        var userPrompt = _promptsGenerator.GetUserPrompt(prompt, category, count);

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

        var requestJson = JsonSerializer.Serialize(request, _jsonSerializerOptions);
        using var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(_settings.DeepSeekApiUrl, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"DeepSeek API error: {error}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<DeepSeekResponse>(responseJson);
        
        if (result?.Choices == null || result.Choices.Count == 0)
            throw new InvalidOperationException("No choices returned from DeepSeek");

        var responseContent = result.Choices.First().Message.Content;
        
        var duelsData = JsonSerializer.Deserialize<Collection<DuelGenerationDto>>(responseContent);

        var validationResult = _duelGenerationValidator.Validate(duelsData);
        
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"Duels generation validation failed: {validationResult.Message}");
        }

        var duelsCreateDtos = duelsData!.Select(d => _duelMapper.Map(d, category.Id, 7));

        return [..duelsCreateDtos];
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
