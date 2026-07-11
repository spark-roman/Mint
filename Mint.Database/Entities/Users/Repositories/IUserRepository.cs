using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Users.Dto;

namespace AdvApplication.Auth.Users;

/// <summary>
/// Repository for working with users
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Create user
    /// </summary>
    /// <param name="user">User entity for creation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user Id</returns>
    Task<long> CreateUserAsync(UserCreateDto user, CancellationToken cancellationToken);

    /// <summary>
    /// Create or update user
    /// </summary>
    /// <param name="user">User entity for creation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created or updated user Id</returns>
    Task<long> CreateOrUpdateUserAsync(UserCreateDto user, CancellationToken cancellationToken);

    /// <summary>
    /// Get user
    /// </summary>
    /// <param name="externalUserId">Vendor user id</param>
    /// <param name="systemType">Auth system type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User</returns>
    Task<UserDto?> GetUserAsync(long externalUserId, byte systemType, CancellationToken cancellationToken);

    /// <summary>
    /// Change user status
    /// </summary>
    /// <param name="externalUserId">Vendor user id</param>
    /// <param name="systemType">Auth system type</param>
    /// <param name="status">New status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user was found and status changed</returns>
    Task<bool> ChangeUserStatusAsync(long externalUserId, byte systemType, UserStatus status, CancellationToken cancellationToken);

    /// <summary>
    /// Gets total number of users with statistics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total count of users</returns>
    Task<int> GetTotalUsersCountAsync(CancellationToken cancellationToken);
}

