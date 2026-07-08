using Mint.Database.Entities.UserInteractive.Bonuses.Dto;

namespace Mint.App.Services.UserInteractive.Bonuses.Rules;

/// <inheritdoc />
public class BonusValidator(TimeProvider timeProvider) : IBonusValidator
{
    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc />
    public Task<bool> CanApplyDailyBonus(UserBonusStatsDto? statsDto, CancellationToken cancellation)
    {
        var now = _timeProvider.GetUtcNow();

        bool validationResult = false;

        if (statsDto is null)
        {
            validationResult = false;
        }
        else if (statsDto.NextDailyAvailableAt is null)
        {
            validationResult = true;
        }
        else if (now >= statsDto.NextDailyAvailableAt.Value)
        {
            validationResult = true;
        }

        return Task.FromResult(validationResult);
    }

    /// <inheritdoc />
    public Task<bool> CanApplyStartBonus(UserBonusStatsDto? statsDto, CancellationToken cancellation)
    {
        return Task.FromResult(statsDto is null || !statsDto.IsStartBonusClaimed);
    }

    /// <inheritdoc />
    public Task<bool> CanApplyStreakBonus(UserBonusStatsDto? statsDto, CancellationToken cancellation)
    {
        var validationResult = statsDto is not null && statsDto.CurrentDailyStreak == 7;

        return Task.FromResult(validationResult);
    }
}
