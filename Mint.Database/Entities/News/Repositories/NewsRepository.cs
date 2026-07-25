using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.News.Dto;

namespace Mint.Database.Entities.News.Repositories;

/// <inheritdoc cref="INewsRepository"/>
public sealed class NewsRepository(
    IDbContextFactory<MintDbContext> dbContextFactory,
    IDbEntityMapper<NewsEntity, NewsDto> mapper,
    IDbEntityMapper<NewsCreateDto, NewsEntity> createMapper) : INewsRepository
{
    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

    private readonly IDbEntityMapper<NewsEntity, NewsDto> _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    private readonly IDbEntityMapper<NewsCreateDto, NewsEntity> _createMapper = createMapper ?? throw new ArgumentNullException(nameof(createMapper));

    /// <inheritdoc />
    public async Task<NewsDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await context.News
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

        return entity != null ? _mapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<NewsDto?> GetByLinkAsync(string link, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await context.News
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Link == link, cancellationToken);

        return entity != null ? _mapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<NewsDto> CreateAsync(NewsCreateDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = _createMapper.Map(dto);
        entity.CreatedAt = DateTimeOffset.UtcNow;

        await context.News.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return _mapper.Map(entity);
    }

    /// <inheritdoc />
    public async Task<List<NewsDto>> CreateManyAsync(Collection<NewsCreateDto> dtos, CancellationToken cancellationToken)
    {
        if (dtos == null || dtos.Count == 0)
            return new List<NewsDto>();

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = dtos.Select(dto =>
        {
            var entity = _createMapper.Map(dto);
            entity.CreatedAt = DateTimeOffset.UtcNow;
            return entity;
        }).ToList();

        await context.News.AddRangeAsync(entities, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entities.Select(_mapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<List<NewsDto>> GetUnprocessedAsync(int limit, string? categoryCode, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = context.News
            .Where(n => !n.IsProcessed)
            .OrderBy(n => n.PublishedAt)
            .Take(limit)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(categoryCode))
        {
            query = query.Where(n => n.CategoryCode == categoryCode);
        }

        var entities = await query.ToListAsync(cancellationToken);

        return entities.Select(_mapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task MarkAsProcessedAsync(Collection<long> ids, CancellationToken cancellationToken)
    {
        if (ids == null || ids.Count == 0)
            return;

        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = await context.News
            .Where(n => ids.Contains(n.Id))
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            entity.IsProcessed = true;
            entity.ProcessedAt = DateTimeOffset.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<NewsDto>> GetRecentAsync(int hours, int limit, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var cutoff = DateTimeOffset.UtcNow.AddHours(-hours);

        var entities = await context.News
            .Where(n => n.PublishedAt > cutoff)
            .OrderByDescending(n => n.PublishedAt)
            .Take(limit)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return entities.Select(_mapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByLinkAsync(string link, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.News
            .AsNoTracking()
            .AnyAsync(n => n.Link == link, cancellationToken);
    }
}
