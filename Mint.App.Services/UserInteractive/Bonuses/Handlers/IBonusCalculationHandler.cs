using Mint.App.Services.UserInteractive.Bonuses.Dto;

namespace Mint.App.Services.UserInteractive.Bonuses.Handlers;

/// <summary>
/// Service for managing all bonus operations.
/// </summary>
public interface IBonusCalculationHandler
{
    /// <summary>
    /// Applies the start bonus if eligible.
    /// </summary>
    Task<BonusResultDto> ApplyStartBonusAsync(
        long externalUserId,
        byte systemType,
        long accountId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Applies the daily bonus if eligible.
    /// </summary>
    Task<BonusResultDto> ApplyDailyBonusAsync(
        long externalUserId,
        byte systemType,
        long accountId,
        CancellationToken cancellationToken);
}
