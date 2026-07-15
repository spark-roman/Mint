using Mint.Database.Entities.UserInteractive.Votes.Dto;

namespace Mint.Database.Entities.UserInteractive.Votes.Repositories;

/// <summary>
/// Repository interface for working with votes
/// </summary>
public interface IVoteRepository
{
    /// <summary>
    /// Create a new vote
    /// </summary>
    /// <param name="dto">DTO for creation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of the created vote</returns>
    Task<long> CreateVoteAsync(VoteCreateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Get vote by duel ID and account ID
    /// </summary>
    /// <param name="duelId">Duel ID</param>
    /// <param name="accountId">Account ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Vote DTO or null if not found</returns>
    Task<VoteDto?> GetVoteAsync(long duelId, long accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Get all votes for a duel
    /// </summary>
    /// <param name="duelId">Duel ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of votes</returns>
    Task<List<VoteDto>?> GetVotesByDuelIdAsync(long duelId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a vote by duel ID and account id.
    /// </summary>
    /// <param name="duelId">Duel id.</param>
    /// <param name="accountId">Account id.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Vote entity</returns>
    Task<VoteEntity?> GetVoteByDuelAndAccountAsync(long duelId, long accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Check if an account has already voted in a duel
    /// </summary>
    /// <param name="duelId">Duel id</param>
    /// <param name="accountId">Account id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>true if vote exists, otherwise false</returns>
    Task<bool> HasAccountVotedAsync(long duelId, long accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Check if an account has already voted in a duel
    /// </summary>
    /// <param name="externalUserId">External user id</param>
    /// <param name="duelId">Duel id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns true if vote exists, otherwise false</returns>
    Task<bool> HasUserVotedInDuelAsync(long externalUserId, long duelId, CancellationToken cancellationToken);

}
