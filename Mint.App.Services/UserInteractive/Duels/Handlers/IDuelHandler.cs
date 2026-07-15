using Mint.App.Services.UserInteractive.Duels.Dto;

namespace Mint.App.Services.UserInteractive.Duels.Handlers;

/// <summary>
/// Provides operations for duel management.
/// </summary>
public interface IDuelHandler
{
    /// <summary>
    /// Gets the first available duel for a category.
    /// </summary>
    Task<DuelCardDto?> GetFirstAvailableDuelAsync(int categoryId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a user has already voted in a duel.
    /// </summary>
    Task<bool> HasUserVotedInDuelAsync(long externalUserId, long duelId, CancellationToken cancellationToken);

    /// <summary>
    /// Places a bet on a duel option.
    /// </summary>
    Task<BetResultDto> PlaceBetAsync(long externalUserId, long duelId, long optionId, decimal amount, CancellationToken cancellationToken);
}
