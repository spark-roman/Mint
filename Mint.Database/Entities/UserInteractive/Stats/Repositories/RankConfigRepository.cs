using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Stats.Dto;

namespace Mint.Database.Entities.UserInteractive.Stats.Repositories;

/// <summary>
/// Repository for rank configs
/// </summary>
/// <param name="rankConfigMapper">Mapper for rank config entity</param>
/// <param name="dbContextFactory">Database context factory</param>
public class RankConfigRepository(
    IDbEntityMapper<RankConfigEntity, RankConfigDto> rankConfigMapper,
    IDbContextFactory<MintDbContext> dbContextFactory) : IRankConfigRepository
{
    private readonly IDbEntityMapper<RankConfigEntity, RankConfigDto> _rankConfigMapper = rankConfigMapper ?? throw new ArgumentNullException(nameof(rankConfigMapper));

    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

    /// <inheritdoc/>
    public async Task<RankConfigDto?> GetRankConfigByIdAsync(int rankId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var rankConfig = await context.RankConfigs
            .FirstOrDefaultAsync(r => r.Id == rankId, cancellationToken);

        return rankConfig is null ? null : _rankConfigMapper.Map(rankConfig);
    }

    /// <inheritdoc/>
    public async Task<RankConfigDto?> GetRankConfigByCodeAsync(string code, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(code);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var rankConfig = await context.RankConfigs
            .FirstOrDefaultAsync(r => r.Code == code, cancellationToken);

        return rankConfig is null ? null : _rankConfigMapper.Map(rankConfig);
    }

    /// <inheritdoc/>
    public async Task<List<RankConfigDto>> GetRankConfigsAsync(CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var rankConfigs = await context.RankConfigs
            .OrderBy(r => r.MinPoints)
            .ToListAsync(cancellationToken);

        return rankConfigs.Select(_rankConfigMapper.Map).ToList();
    }

    /// <inheritdoc/>
    public async Task<RankConfigDto?> GetHighestRankAsync(CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var rankConfig = await context.RankConfigs
            .OrderByDescending(r => r.MinPoints)
            .FirstOrDefaultAsync(cancellationToken);

        return rankConfig is null ? null : _rankConfigMapper.Map(rankConfig);
    }
}
