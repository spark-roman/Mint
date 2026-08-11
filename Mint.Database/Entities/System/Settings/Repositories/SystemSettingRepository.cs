using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.System.Settings.Dto;

namespace Mint.Database.Entities.System.Settings.Repositories;

/// <inheritdoc cref="ISystemSettingRepository"/>
public sealed class SystemSettingRepository(
    IDbContextFactory<MintDbContext> dbContextFactory,
    IDbEntityMapper<SystemSettingEntity, SystemSettingDto> mapper,
    IDbEntityMapper<SystemSettingUpsertDto, SystemSettingEntity> createMapper) : ISystemSettingRepository
{
    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory
        ?? throw new ArgumentNullException(nameof(dbContextFactory));

    private readonly IDbEntityMapper<SystemSettingEntity, SystemSettingDto> _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));

    private readonly IDbEntityMapper<SystemSettingUpsertDto, SystemSettingEntity> _createMapper = createMapper
        ?? throw new ArgumentNullException(nameof(createMapper));

    /// <inheritdoc/>
    public async Task<SystemSettingDto?> GetByKeyAsync(string key, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var entity = await context.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key, ct);

        return entity != null ? _mapper.Map(entity) : null;
    }

    /// <inheritdoc/>
    public async Task<List<SystemSettingDto>> GetAllAsync(CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var entities = await context.SystemSettings
            .AsNoTracking()
            .ToListAsync(ct);

        return entities.Select(_mapper.Map).ToList();
    }

    /// <inheritdoc/>
    public async Task<SystemSettingDto> UpsertAsync(SystemSettingUpsertDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        var existing = await context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == dto.Key, ct);

        if (existing != null)
        {
            existing.Value = dto.Value;
            existing.Description = dto.Description;
            existing.UpdatedAt = DateTimeOffset.UtcNow;

            context.SystemSettings.Update(existing);
            await context.SaveChangesAsync(ct);

            return _mapper.Map(existing);
        }

        var entity = _createMapper.Map(dto);
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await context.SystemSettings.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);

        return _mapper.Map(entity);
    }

    /// <inheritdoc/>
    public async Task<decimal> GetDecimalAsync(string key, decimal defaultValue, CancellationToken ct)
    {
        var setting = await GetByKeyAsync(key, ct);
        return setting != null && decimal.TryParse(setting.Value, out var result) ? result : defaultValue;
    }

    /// <inheritdoc/>
    public async Task<int> GetIntAsync(string key, int defaultValue, CancellationToken ct)
    {
        var setting = await GetByKeyAsync(key, ct);
        return setting != null && int.TryParse(setting.Value, out var result) ? result : defaultValue;
    }
}
