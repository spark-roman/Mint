using Mint.Common.Contracts.UserActions.Bonuses.Rules;

namespace Mint.App.Services.UserInteractive.Bonuses.Rules;

/// <summary>
/// Streak bonus validator.
/// </summary>
public class StreakBonusValidator : IBonusValidator
{
    /// <inheritdoc />
    public Task<bool> Validate(long userId, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
