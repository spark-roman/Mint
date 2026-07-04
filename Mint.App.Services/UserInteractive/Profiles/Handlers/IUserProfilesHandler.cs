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
    /// Gets a user's profile.
    /// </summary>
    /// <param name="userDto">External user dto.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns user profile</returns>
    Task<UserProfileDto?> GetUserProfileAsync(ExternalUserDto userDto, CancellationToken cancellationToken);

    /// <summary>
    /// Initializes a new user (creates account, stats, bonus records).
    /// </summary>
    /// <param name="userCreateDto">Dto for creating a new user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User DTO.</returns>
    Task<UserDto> InitializeUserAsync(UserCreateDto userCreateDto, CancellationToken cancellationToken);

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

    /// <summary>
    /// Gets top ranked users for leaderboard.
    /// </summary>
    /// <param name="limit">Maximum number of users to return.</param>
    /// <param name="systemType">Auth system type.</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>List of leaderboard entries.</returns>
    Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int limit, AuthSystem systemType, CancellationToken cancellationToken);
}
