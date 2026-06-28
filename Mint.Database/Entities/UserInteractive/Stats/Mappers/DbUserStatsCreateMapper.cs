using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Stats.Dto;

namespace Mint.Database.Entities.UserInteractive.Stats.Mappers;

/// <summary>
/// Mapper for creating user stats entity
/// </summary>
public class DbUserStatsCreateMapper : IDbEntityMapper<UserStatsCreateDto, UserStatsEntity>
{
    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="timeProvider">Time provider</param>
    public DbUserStatsCreateMapper(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    private readonly TimeProvider _timeProvider;

    /// <inheritdoc/>
    public UserStatsEntity Map(UserStatsCreateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new UserStatsEntity
        {
            UserId = entity.UserId,
            RankPoints = entity.RankPoints,
            TotalWins = entity.TotalWins,
            TotalLosses = entity.TotalLosses,
            UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime
        };
    }
}
