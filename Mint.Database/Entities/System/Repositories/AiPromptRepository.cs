using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.System.Dto;

namespace Mint.Database.Entities.System.Repositories;

/// <summary>
/// Repository for AI prompts
/// </summary>
/// <param name="aiPromptCreateMapper">Mapper for creating AI prompt</param>
/// <param name="aiPromptMapper">Mapper for AI prompt entity</param>
/// <param name="dbContextFactory">Database context factory</param>
public class AiPromptRepository(
    IDbEntityMapper<AiPromptCreateDto, AiPromptEntity> aiPromptCreateMapper,
    IDbEntityMapper<AiPromptEntity, AiPromptDto> aiPromptMapper,
    IDbContextFactory<MintDbContext> dbContextFactory) : IAiPromptRepository
{
    private readonly IDbEntityMapper<AiPromptCreateDto, AiPromptEntity> _aiPromptCreateMapper = aiPromptCreateMapper ?? throw new ArgumentNullException(nameof(aiPromptCreateMapper));

    private readonly IDbEntityMapper<AiPromptEntity, AiPromptDto> _aiPromptMapper = aiPromptMapper ?? throw new ArgumentNullException(nameof(aiPromptMapper));

    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

    /// <inheritdoc/>
    public async Task<int> CreateOrUpdateAsync(AiPromptCreateDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existingPrompt = await context.AiPrompts.FirstOrDefaultAsync(cancellationToken);

        var entity = _aiPromptCreateMapper.Map(dto);

        if (existingPrompt is not null)
        {
            existingPrompt.SystemPromptTemplate = entity.SystemPromptTemplate;
            existingPrompt.UserPromptTemplate = entity.UserPromptTemplate;
            existingPrompt.Temperature = entity.Temperature;
            existingPrompt.MaxDuelsPerRun = entity.MaxDuelsPerRun;
            existingPrompt.UpdatedAt = entity.UpdatedAt;
        }
        else
        {
            await context.AiPrompts.AddAsync(entity, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        return existingPrompt is not null ? existingPrompt.Id : entity.Id;
    }

    /// <inheritdoc/>
    public async Task<AiPromptDto?> GetPromptAsync(long promptId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var prompt = context.AiPrompts
            .Include(p => p.Categories.Where(c => c.IsActiveForAI))
            .Select(_aiPromptMapper.Map)
            .FirstOrDefault();

        return prompt;
    }

    /// <inheritdoc/>
    public async Task<List<AiPromptDto>> GetPromptsAsync(CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var prompts = context.AiPrompts
            .Include(p => p.Categories.Where(c => c.IsActiveForAI))
            .Select(_aiPromptMapper.Map)
            .ToList();

        return prompts;
    }
}
