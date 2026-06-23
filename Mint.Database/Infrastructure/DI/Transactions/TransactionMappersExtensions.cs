using Mint.Common.Contracts.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.Transactions;
using Mint.Database.Entities.Transactions.Dto;
using Mint.Database.Entities.Transactions.Mappers;

namespace Mint.Database.Infrastructure.DI.Transactions;

/// <summary>
/// Extension methods for transaction entity
/// </summary>
public static class TransactionMappersExtensions
{
    /// <summary>
    /// Register transaction mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterTransactionMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDbEntityMapper<TransactionCreateDto, TransactionEntity>, DbTransactionCreateMapper>();
        services.AddSingleton<IDbEntityMapper<TransactionEntity, TransactionDto>, DbTransactionMapper>();
    }
}
