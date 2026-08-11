namespace Mint.Database.Entities.System.Settings.Dto;

/// <summary>
/// DTO for creating or updating a system setting.
/// </summary>
public sealed record SystemSettingUpsertDto
{
    /// <summary>
    /// Key of the setting.
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// Value of the setting.
    /// </summary>
    public required string Value { get; init; }

    /// <summary>
    /// Description of the setting.
    /// </summary>
    public string? Description { get; init; }
}
