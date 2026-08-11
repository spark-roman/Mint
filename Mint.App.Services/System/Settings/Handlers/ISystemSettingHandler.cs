using Mint.Database.Entities.System.Settings.Dto;

namespace Mint.App.Services.System.Settings.Handlers;

/// <summary>
/// Handles system settings with caching.
/// </summary>
public interface ISystemSettingHandler
{
    /// <summary>Gets a setting by its key (cached).</summary>
    Task<SystemSettingDto?> GetByKeyAsync(string key, CancellationToken ct);

    /// <summary>Gets all settings (cached).</summary>
    Task<List<SystemSettingDto>> GetAllAsync(CancellationToken ct);

    /// <summary>Creates or updates a setting (clears cache).</summary>
    Task<SystemSettingDto> UpsertAsync(SystemSettingUpsertDto dto, CancellationToken ct);

    /// <summary>Gets the value as a decimal (cached).</summary>
    Task<decimal> GetDecimalAsync(string key, decimal defaultValue, CancellationToken ct);

    /// <summary>Gets the value as an integer (cached).</summary>
    Task<int> GetIntAsync(string key, int defaultValue, CancellationToken ct);
}