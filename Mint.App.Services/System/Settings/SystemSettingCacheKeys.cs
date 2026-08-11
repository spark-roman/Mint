namespace Mint.App.Services.System.Settings;

/// <summary>
/// Contains cache keys for system settings.
/// </summary>
public static class SystemSettingCacheKeys
{
    /// <summary>Cache key for all settings.</summary>
    public const string All = "system_settings_all";

    /// <summary>Cache key prefix for a single setting.</summary>
    public const string Prefix = "system_setting_";

    /// <summary>Gets cache key for a specific setting key.</summary>
    public static string GetKey(string key) => $"{Prefix}{key}";
}
