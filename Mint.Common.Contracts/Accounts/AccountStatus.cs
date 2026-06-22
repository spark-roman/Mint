namespace Mint.Common.Contracts.Accounts;

/// <summary>
/// Account status
/// </summary>
[Flags]
public enum AccountStatus
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,

    /// <summary>
    /// Active
    /// </summary>
    Active = 1,

    /// <summary>
    /// Inactive
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Deleted
    /// </summary>
    Deleted = 4
}
