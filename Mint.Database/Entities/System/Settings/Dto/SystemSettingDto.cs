namespace Mint.Database.Entities.System.Settings.Dto;

/// <summary>
/// Data transfer object for system setting.
/// </summary>
public sealed record SystemSettingDto
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Key of the setting
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// Value of the setting
    /// </summary>
    public required string Value { get; init; }

    /// <summary>
    /// Description of the setting
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Last update date time
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}
