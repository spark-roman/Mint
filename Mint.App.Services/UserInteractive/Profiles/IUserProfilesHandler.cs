using Mint.App.Services.UserInteractive.Profiles.Dto;
using Mint.App.Services.UserInteractive.Users.Dto;

namespace Mint.App.Services.UserInteractive.Profiles;

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
}
