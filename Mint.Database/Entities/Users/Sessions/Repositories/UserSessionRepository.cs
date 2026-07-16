using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Users.Sessions.Dto;

namespace Mint.Database.Entities.Users.Sessions.Repositories;

/// <inheritdoc cref="IUserSessionRepository"/>
/// <summary>
/// Constructor.
/// </summary>
public sealed class UserSessionRepository(
    IDbContextFactory<MintDbContext> dbContextFactory,
    IDbEntityMapper<UserSessionEntity, UserSessionDto> sessionMapper) : IUserSessionRepository
{
    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory;
    private readonly IDbEntityMapper<UserSessionEntity, UserSessionDto> _sessionMapper = sessionMapper;

    /// <inheritdoc />
    public async Task<UserSessionDto?> GetActiveSessionAsync(long externalUserId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await context.UserSessions
            .OrderBy(s => s.StartedAt)
            .Include(us => us.Scenario)
            .Include(us => us.CurrentStep)
                .ThenInclude(st => st!.Buttons)
            .AsNoTracking()
            .LastOrDefaultAsync(us => 
                us.CompletedAt == null &&
                us.User.ExternalUserId == externalUserId, cancellationToken);

        return entity != null ? _sessionMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<UserSessionDto?> GetSessionByUserIdAndScenarioAsync(long externalUserId, long scenarioId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await context.UserSessions
            .Include(us => us.CurrentStep)
                .ThenInclude(st => st!.Buttons)
            .AsNoTracking()
            .FirstOrDefaultAsync(us => 
                us.ScenarioId == scenarioId && 
                us.CompletedAt == null &&
                us.User.ExternalUserId == externalUserId, cancellationToken);

        return entity != null ? _sessionMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<UserSessionDto> CreateOrUpdateSessionAsync(long externalUserId, long scenarioId, long stepId, string data, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(data);
        
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.ExternalUserId == externalUserId, cancellationToken);
        
        if (user == null)
        {
            throw new InvalidOperationException($"User with ExternalUserId {externalUserId} not found");
        }
        
        var existing = await context.UserSessions
            .FirstOrDefaultAsync(us => 
                us.UserId == user.Id && 
                us.ScenarioId == scenarioId && 
                us.CompletedAt == null, cancellationToken);

        UserSessionEntity entity;

        if (existing != null)
        {
            existing.CurrentStepId = stepId;
            existing.Data = data;
            existing.StartedAt = DateTimeOffset.UtcNow;
            existing.CompletedAt = null;

            context.UserSessions.Update(existing);
            await context.SaveChangesAsync(cancellationToken);
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

            await context.UserSessions.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        return _sessionMapper.Map(entity);
    }

    /// <inheritdoc />
    public async Task<UserSessionDto> UpdateCurrentStepAsync(long sessionId, long stepId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await context.UserSessions
            .FirstOrDefaultAsync(us => us.Id == sessionId, cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"Session {sessionId} not found");
        }

        entity.CurrentStepId = stepId;
        context.UserSessions.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return _sessionMapper.Map(entity);
    }

    /// <inheritdoc />
    public async Task<UserSessionDto> UpdateSessionDataAsync(long sessionId, string data, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await context.UserSessions
            .FirstOrDefaultAsync(us => us.Id == sessionId, cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"Session {sessionId} not found");
        }

        entity.Data = data;
        context.UserSessions.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return _sessionMapper.Map(entity);
    }

    /// <inheritdoc />
    public async Task CompleteSessionAsync(long sessionId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await context.UserSessions
            .FirstOrDefaultAsync(us => us.Id == sessionId, cancellationToken);

        if (entity != null)
        {
            entity.CompletedAt = DateTimeOffset.UtcNow;
            context.UserSessions.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task DeleteSessionAsync(long sessionId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await context.UserSessions
            .FirstOrDefaultAsync(us => us.Id == sessionId, cancellationToken);

        if (entity != null)
        {
            context.UserSessions.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}