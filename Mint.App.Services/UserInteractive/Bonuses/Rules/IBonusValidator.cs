using Mint.Database.Entities.UserInteractive.Bonuses.Dto;

namespace Mint.App.Services.UserInteractive.Bonuses.Rules;

/// <summary>
/// Validator for bonus rules.
/// </summary>
public interface IBonusValidator
{
    /// <summary>
    /// Checks if user can apply daily bonus.
    /// </summary>
    /// <param name="statsDto">User bonus stats.</param>
    /// <param name="cancellation">Cancellation token</param>
    Task<bool> CanApplyDailyBonus(UserBonusStatsDto? statsDto, CancellationToken cancellation);

    /// <summary>
    /// Checks if user can apply start bonus.
    /// </summary>
    /// <param name="statsDto">User bonus stats.</param>
    /// <param name="cancellation">Cancellation token</param>
    Task<bool> CanApplyStartBonus(UserBonusStatsDto? statsDto, CancellationToken cancellation);

    /// <summary>
    /// Checks if user can apply streak bonus.
    /// </summary>
    /// <param name="statsDto">User bonus stats.</param>
    /// <param name="cancellation">Cancellation token</param>
    Task<bool> CanApplyStreakBonus(UserBonusStatsDto? statsDto, CancellationToken cancellation);
}
