namespace Mint.Common.Contracts.UserActions.Bonuses.Rules;

/// <summary>
/// Validator for bonus rules.
/// </summary>
public interface IBonusValidator
{
    /// <summary>
    /// Validates a specific bonus rule.
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="cancellation">Cancellation token</param>
    Task<bool> Validate(long userId, CancellationToken cancellation);
}
