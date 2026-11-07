using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.News.RSS.Dto;

namespace Mint.Database.Entities.News.RSS.Repositories;

/// <inheritdoc cref="IRssSourceRepository"/>
public sealed class RssSourceRepository(
    IDbContextFactory<MintDbContext> dbContextFactory,
    IDbEntityMapper<RssSourceEntity, RssSourceDto> mapper) : IRssSourceRepository
{
    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    private readonly IDbEntityMapper<RssSourceEntity, RssSourceDto> _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    /// <inheritdoc/>
    public async Task<List<RssSourceDto>> GetActiveSourcesAsync(CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = await context.RssSources
            .Where(s => s.IsActive)
            .OrderByDescending(s => s.Priority)
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
}
