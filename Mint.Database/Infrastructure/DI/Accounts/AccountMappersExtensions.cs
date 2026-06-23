using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Accounts.Dto;
using Mint.Database.Entities.Ledger.Accounts.Mappers;

namespace Mint.Database.Infrastructure.DI.Accounts;

/// <summary>
/// Extension methods for account entity
/// </summary>
public static class AccountMappersExtensions
{
    /// <summary>
    /// Register account mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterAccountMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDbEntityMapper<AccountCreateDto, AccountEntity>, DbAccountCreateMapper>();
        services.AddSingleton<IDbEntityMapper<AccountEntity, AccountDto>, DbAccountMapper>();
    }
}
