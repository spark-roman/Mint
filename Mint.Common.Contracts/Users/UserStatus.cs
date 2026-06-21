namespace Mint.Common.Contracts.Users;

/// <summary>
/// User status
/// </summary>
[Flags]
public enum UserStatus
{
    /// <summary>
    /// Undefined
    /// </summary>
    None = 0,

    /// <summary>
    /// User is inactive
    /// </summary>
    Active = 1,

    /// <summary>
    /// User is blocked
    /// </summary>
    Blocked = 2,

    /// <summary>
    /// User is deleted
    /// </summary>
    Deleted = 4
}
