using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Users.Sessions.Dto;

namespace Mint.Database.Entities.Users.Sessions.Repositories;

/// <inheritdoc cref="IUserSessionRepository"/>
public sealed class UserSessionRepository : IUserSessionRepository
{
    private readonly IDbContextFactory<MintDbContext> _dbContextFactory;
    private readonly IDbEntityMapper<UserSessionEntity, UserSessionDto> _sessionMapper;
    private readonly IDbEntityMapper<UserSessionDto, UserSessionEntity> _sessionCreateMapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    public UserSessionRepository(
        IDbContextFactory<MintDbContext> dbContextFactory,
        IDbEntityMapper<UserSessionEntity, UserSessionDto> sessionMapper,
        IDbEntityMapper<UserSessionDto, UserSessionEntity> sessionCreateMapper)
    {
        _dbContextFactory = dbContextFactory;
        _sessionMapper = sessionMapper;
        _sessionCreateMapper = sessionCreateMapper;
    }

    /// <inheritdoc />
    public async Task<UserSessionDto?> GetActiveSessionAsync(long externalUserId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entity = await context.UserSessions
            .Include(us => us.Scenario)
            .Include(us => us.CurrentStep)
                .ThenInclude(st => st!.Buttons)
            .AsNoTracking()
            .FirstOrDefaultAsync(us => 
                us.CompletedAt == null &&
                us.User.ExternalUserId == externalUserId);

        return entity != null ? _sessionMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<UserSessionDto?> GetSessionByUserIdAndScenarioAsync(long externalUserId, long scenarioId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entity = await context.UserSessions
            .Include(us => us.CurrentStep)
                .ThenInclude(st => st!.Buttons)
            .AsNoTracking()
            .FirstOrDefaultAsync(us => 
                us.ScenarioId == scenarioId && 
                us.CompletedAt == null &&
                us.User.ExternalUserId == externalUserId);

        return entity != null ? _sessionMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<UserSessionDto> CreateOrUpdateSessionAsync(long externalUserId, long scenarioId, long stepId, string data = "{}")
    {
        ArgumentNullException.ThrowIfNull(data);
        
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        // Находим внутреннего пользователя
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.ExternalUserId == externalUserId);
        
        if (user == null)
        {
            throw new InvalidOperationException($"User with ExternalUserId {externalUserId} not found");
        }
        
        var existing = await context.UserSessions
            .FirstOrDefaultAsync(us => 
                us.UserId == user.Id && 
                us.ScenarioId == scenarioId && 
                us.CompletedAt == null);

        UserSessionEntity entity;

        if (existing != null)
        {
            existing.CurrentStepId = stepId;
            existing.Data = data;
            existing.StartedAt = DateTimeOffset.UtcNow;
            existing.CompletedAt = null;

            context.UserSessions.Update(existing);
            await context.SaveChangesAsync();
            entity = existing;
        }
        else
        {
            entity = new UserSessionEntity
            {
                UserId = user.Id,
                ScenarioId = scenarioId,
                CurrentStepId = stepId,
                Data = data,
                StartedAt = DateTimeOffset.UtcNow
            };

            await context.UserSessions.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        return _sessionMapper.Map(entity);
    }

    /// <inheritdoc />
    public async Task<UserSessionDto> UpdateCurrentStepAsync(long sessionId, long stepId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entity = await context.UserSessions
            .FirstOrDefaultAsync(us => us.Id == sessionId);

        if (entity == null)
        {
            throw new InvalidOperationException($"Session {sessionId} not found");
        }

        entity.CurrentStepId = stepId;
        context.UserSessions.Update(entity);
        await context.SaveChangesAsync();

        return _sessionMapper.Map(entity);
    }

    /// <inheritdoc />
    public async Task<UserSessionDto> UpdateSessionDataAsync(long sessionId, string data)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entity = await context.UserSessions
            .FirstOrDefaultAsync(us => us.Id == sessionId);

        if (entity == null)
        {
            throw new InvalidOperationException($"Session {sessionId} not found");
        }

        entity.Data = data;
        context.UserSessions.Update(entity);
        await context.SaveChangesAsync();

        return _sessionMapper.Map(entity);
    }

    /// <inheritdoc />
    public async Task CompleteSessionAsync(long sessionId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entity = await context.UserSessions
            .FirstOrDefaultAsync(us => us.Id == sessionId);

        if (entity != null)
        {
            entity.CompletedAt = DateTimeOffset.UtcNow;
            context.UserSessions.Update(entity);
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task DeleteSessionAsync(long sessionId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entity = await context.UserSessions
            .FirstOrDefaultAsync(us => us.Id == sessionId);

        if (entity != null)
        {
            context.UserSessions.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}