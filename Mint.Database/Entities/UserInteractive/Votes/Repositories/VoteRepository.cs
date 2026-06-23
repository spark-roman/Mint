using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Votes.Dto;

namespace Mint.Database.Entities.UserInteractive.Votes.Repositories;

/// <summary>
/// Repository for votes
/// </summary>
/// <param name="voteCreateMapper">Mapper for creating vote</param>
/// <param name="voteMapper">Mapper for vote entity</param>
/// <param name="dbContextFactory">Database context factory</param>
public class VoteRepository(
    IDbEntityMapper<VoteCreateDto, VoteEntity> voteCreateMapper,
    IDbEntityMapper<VoteEntity, VoteDto> voteMapper,
    IDbContextFactory<MintDbContext> dbContextFactory) : IVoteRepository
{
    private readonly IDbEntityMapper<VoteCreateDto, VoteEntity> _voteCreateMapper = voteCreateMapper ?? throw new ArgumentNullException(nameof(voteCreateMapper));

    private readonly IDbEntityMapper<VoteEntity, VoteDto> _voteMapper = voteMapper ?? throw new ArgumentNullException(nameof(voteMapper));

    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

    /// <inheritdoc/>
    public async Task<long> CreateVoteAsync(VoteCreateDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = _voteCreateMapper.Map(dto);

        await context.Votes.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity.DuelId;
    }

    /// <inheritdoc/>
    public async Task<VoteDto?> GetVoteAsync(long duelId, long accountId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var vote = await context.Votes
            .FirstOrDefaultAsync(v => v.DuelId == duelId && v.AccountId == accountId, cancellationToken);

        return vote is null ? null : _voteMapper.Map(vote);
    }

    /// <inheritdoc/>
    public async Task<List<VoteDto>?> GetVotesByDuelIdAsync(long duelId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = await context.Votes
            .Where(v => v.DuelId == duelId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(_voteMapper.Map).ToList();
    }

    /// <inheritdoc/>
    public async Task<bool> HasAccountVotedAsync(long duelId, long accountId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await context.Votes
            .AnyAsync(v => v.DuelId == duelId && v.AccountId == accountId, cancellationToken);
    }
}
