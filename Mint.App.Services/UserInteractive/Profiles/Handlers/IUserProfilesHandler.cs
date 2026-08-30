using Mint.App.Services.UserInteractive.Profiles.Dto;
using Mint.App.Services.UserInteractive.Users.Dto;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Users.Dto;

namespace Mint.App.Services.UserInteractive.Profiles.Handlers;

/// <summary>
/// Handles user profile operations.
/// </summary>
public interface IUserProfilesHandler
{
    /// <summary>
    /// Initializes a new user (creates account, stats, bonus records).
    /// </summary>
    /// <param name="userCreateDto">Dto for creating a new user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User DTO.</returns>
    Task<UserDto> InitializeUserAsync(UserCreateDto userCreateDto, CancellationToken cancellationToken);

    /// <summary>
    /// Processes a referral code.
    /// </summary>
    /// <param name="newUserId">New user identifier.</param>
    /// <param name="referralCode">Referral code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ProcessReferralAsync(long newUserId, string referralCode, CancellationToken cancellationToken);

    /// <summary>
    /// Gets user profile with all related data (balance, stats, bonus, rank).
    /// </summary>
    /// <param name="externalUserId">Telegram user identifier.</param>
    /// <param name="systemType">System type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User profile DTO.</returns>
    Task<UserProfileDto> GetProfileAsync(long externalUserId, AuthSystem systemType, CancellationToken cancellationToken);

    /// <summary>
    /// Claims daily bonus for a user.
    /// </summary>
    /// <param name="externalUserId">Telegram user identifier.</param>
    /// <param name="systemType">System type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if bonus was claimed, false if not available.</returns>
    Task<bool> ClaimDailyBonusAsync(long externalUserId, AuthSystem systemType, CancellationToken cancellationToken);
}
