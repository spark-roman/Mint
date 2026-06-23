using Mint.Common.Contracts.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Mappers;
using Mint.Database.Entities.UserInteractive.Votes;

namespace Mint.Database.Infrastructure.DI.UserInteractive;

/// <summary>
/// Extension methods for vote mappers
/// </summary>
public static class VoteMappersExtensions
{
    /// <summary>
    /// Register vote mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterVoteMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDbEntityMapper<VoteCreateDto, VoteEntity>, DbVoteCreateMapper>();
        services.AddSingleton<IDbEntityMapper<VoteEntity, VoteDto>, DbVoteMapper>();
    }
}
