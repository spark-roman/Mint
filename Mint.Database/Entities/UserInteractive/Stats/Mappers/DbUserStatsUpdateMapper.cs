using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Stats.Dto;

namespace Mint.Database.Entities.UserInteractive.Stats.Mappers;

/// <summary>
/// Mapper for updating user stats entity
/// </summary>
public class DbUserStatsUpdateMapper : IDbEntityMapper<UserStatsUpdateDto, UserStatsEntity>
{
    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="timeProvider">Time provider</param>
    public DbUserStatsUpdateMapper(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    private readonly TimeProvider _timeProvider;

    /// <inheritdoc/>
    public UserStatsEntity Map(UserStatsUpdateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new UserStatsEntity
        {
            RankPoints = entity.RankPoints,
            TotalWins = entity.TotalWins,
            TotalLosses = entity.TotalLosses,
            UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime
        };
    }
}
