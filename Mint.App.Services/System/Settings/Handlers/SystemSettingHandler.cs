using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Mint.Database.Entities.System.Settings.Dto;
using Mint.Database.Entities.System.Settings.Repositories;

namespace Mint.App.Services.System.Settings.Handlers;

/// <inheritdoc cref="ISystemSettingHandler"/>
public sealed class SystemSettingHandler(
    ISystemSettingRepository repository,
    IMemoryCache cache,
    ILogger<SystemSettingHandler> logger) : ISystemSettingHandler
{
    private readonly ISystemSettingRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ILogger<SystemSettingHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<SystemSettingDto?> GetByKeyAsync(string key, CancellationToken ct)
    {
        var cacheKey = SystemSettingCacheKeys.GetKey(key);

        if (_cache.TryGetValue(cacheKey, out SystemSettingDto? cached))
        {
            _logger.LogDebug("Cache hit for setting: {Key}", key);
            return cached;
        }

        _logger.LogDebug("Cache miss for setting: {Key}", key);

        var dto = await _repository.GetByKeyAsync(key, ct);
        if (dto == null)
            return null;

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            _cache.Set(cacheKey, dto, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                SlidingExpiration = TimeSpan.FromHours(1),
                Size = 1
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set cache for setting: {Key}", key);
        }
#pragma warning restore CA1031 // Do not catch general exception types

        return dto;
    }

    /// <inheritdoc />
    public async Task<List<SystemSettingDto>> GetAllAsync(CancellationToken ct)
    {
        if (_cache.TryGetValue(SystemSettingCacheKeys.All, out List<SystemSettingDto>? cached))
        {
            _logger.LogDebug("Cache hit for all settings");
            return cached!;
        }

        _logger.LogDebug("Cache miss for all settings");

        var dtos = await _repository.GetAllAsync(ct);

        _cache.Set(SystemSettingCacheKeys.All, dtos, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
            SlidingExpiration = TimeSpan.FromHours(1)
        });

        return dtos;
    }

    /// <inheritdoc />
    public async Task<SystemSettingDto> UpsertAsync(SystemSettingUpsertDto dto, CancellationToken ct)
    {
        var result = await _repository.UpsertAsync(dto, ct);

        ClearCache();

        return result;
    }

    /// <inheritdoc />
    public async Task<decimal> GetDecimalAsync(string key, decimal defaultValue, CancellationToken ct)
    {
        var setting = await GetByKeyAsync(key, ct);
        if (setting == null || !decimal.TryParse(setting.Value, out var result))
        {
            _logger.LogWarning("Setting {Key} not found or invalid decimal, using default: {Default}", key, defaultValue);
            return defaultValue;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<int> GetIntAsync(string key, int defaultValue, CancellationToken ct)
    {
        var setting = await GetByKeyAsync(key, ct);
        if (setting == null || !int.TryParse(setting.Value, out var result))
        {
            _logger.LogWarning("Setting {Key} not found or invalid int, using default: {Default}", key, defaultValue);
            return defaultValue;
        }

        return result;
    }

    private void ClearCache()
    {
        _logger.LogInformation("Clearing system settings cache");
        _cache.Remove(SystemSettingCacheKeys.All);
    }
}
