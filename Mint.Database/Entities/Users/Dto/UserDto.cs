using Mint.Common.Contracts.Users;

namespace Mint.Database.Entities.Users.Dto;

/// <summary>
/// User dto for repository using
/// </summary>
public record UserDto
{
    /// <summary>
    /// Internal user id
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Vendor system UserId
    /// </summary>
    public long ExternalUserId { get; init; }

    /// <summary>
    /// Auth system type
    /// </summary>
    public byte SystemType { get; init; }

    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// User name
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// Last auth date
    /// </summary>
    public DateTimeOffset? LastAuthDate { get; init; }

    /// <summary>
    /// User creation date
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// User status
    /// </summary>
    public UserStatus Status { get; init; }
}