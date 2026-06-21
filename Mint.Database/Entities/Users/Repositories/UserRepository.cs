
using System.ComponentModel.DataAnnotations;
using AdvApplication.Auth.Users;
using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Users.Dto;

namespace Mint.Database.Entities.Users.Repositories;

/// <inheritdoc/>
public class UserRepository : IUserRepository
{
    private readonly IDbEntityMapper<UserCreateDto, UserEntity> _userCreateMapper;

    private readonly IDbEntityMapper<UserEntity, UserDto> _userMapper;

    private readonly IDbContextFactory<MintDbContext> _dbContextFactory;

    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="userCreateMapper">Create user mapper</param>
    /// <param name="userMapper">Mapper for user entity</param>
    /// <param name="dbContextFactory">Database context factory</param>
    /// <param name="timeProvider">Date/time provider</param>
    public UserRepository(
        IDbEntityMapper<UserCreateDto, UserEntity> userCreateMapper,
        IDbEntityMapper<UserEntity, UserDto> userMapper,
        IDbContextFactory<MintDbContext> dbContextFactory,
        TimeProvider timeProvider)
    {
        _userMapper = userMapper ?? throw new ArgumentNullException(nameof(userMapper));
        _userCreateMapper = userCreateMapper ?? throw new ArgumentNullException(nameof(userCreateMapper));
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    /// <inheritdoc/>
    public async Task<long> CreateUserAsync(UserCreateDto user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = _userCreateMapper.Map(user);

        await context.Users.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    /// <inheritdoc/>
    public async Task<UserDto?> GetUserAsync([Required] long externalUserId, [Required] byte systemType, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.ExternalUserId == externalUserId && u.SystemType == systemType, cancellationToken);

        return user is null ? null : _userMapper.Map(user);
    }

    /// <inheritdoc/>
    public async Task<bool> ChangeUserStatusAsync(long externalUserId, byte systemType, UserStatus status, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.ExternalUserId == externalUserId && u.SystemType == systemType, cancellationToken);

        if (user is null)
        {
            return false;
        }

        user.Status = (byte)status;
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

