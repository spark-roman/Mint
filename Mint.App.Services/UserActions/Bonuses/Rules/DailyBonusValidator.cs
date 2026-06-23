using Mint.Common.Contracts.UserActions.Bonuses.Rules;

namespace Mint.App.Services.UserActions.Bonuses.Rules;

/// <summary>
/// Daily bonus validator.
/// </summary>
public class DailyBonusValidator : IBonusValidator
{
    /// <inheritdoc />
    public Task<bool> Validate(long userId, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
