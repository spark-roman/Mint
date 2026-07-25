using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.News.RSS.Dto;

namespace Mint.Database.Entities.News.RSS.Repositories;

/// <inheritdoc cref="IRssSourceRepository"/>
/// <summary>
/// 
/// </summary>
/// <param name="dbContextFactory"></param>
/// <param name="mapper"></param>
/// <param name="createMapper"></param>
public sealed class RssSourceRepository(
    IDbContextFactory<MintDbContext> dbContextFactory,
    IDbEntityMapper<RssSourceEntity, RssSourceDto> mapper,
    IDbEntityMapper<RssSourceCreateDto, RssSourceEntity> createMapper) : IRssSourceRepository
{
    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    
    private readonly IDbEntityMapper<RssSourceEntity, RssSourceDto> _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    private readonly IDbEntityMapper<RssSourceCreateDto, RssSourceEntity> _createMapper = createMapper ?? throw new ArgumentNullException(nameof(createMapper));

    /// <inheritdoc/>
    public async Task<List<RssSourceDto>> GetActiveSourcesAsync(CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = await context.RssSources
            .Where(s => s.IsActive)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return entities.Select(_mapper.Map).ToList();
    }

    /// <inheritdoc/>
    public async Task<RssSourceDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await context.RssSources
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return entity != null ? _mapper.Map(entity) : null;
    }

    /// <inheritdoc/>
    public async Task<RssSourceDto?> GetByUrlAsync(Uri url, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await context.RssSources
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Url == url, cancellationToken);

        return entity != null ? _mapper.Map(entity) : null;
    }

    /// <inheritdoc/>
    public async Task<RssSourceDto> CreateAsync(RssSourceCreateDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = _createMapper.Map(dto);
        entity.CreatedAt = DateTimeOffset.UtcNow;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await context.RssSources.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return _mapper.Map(entity);
    }

    /// <inheritdoc/>
    public async Task<RssSourceDto> UpdateAsync(RssSourceDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await context.RssSources.FirstOrDefaultAsync(s => s.Id == dto.Id, cancellationToken);

        if (entity == null)
            throw new InvalidOperationException($"RSS source with ID {dto.Id} not found");

        entity.Name = dto.Name;
        entity.Url = dto.Url;
        entity.CategoryCode = dto.CategoryCode;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return _mapper.Map(entity);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await context.RssSources
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (entity != null)
        {
            context.RssSources.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
