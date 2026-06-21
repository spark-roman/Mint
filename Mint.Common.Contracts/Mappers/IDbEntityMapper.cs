namespace Mint.Common.Contracts.Mappers;

/// <summary>
/// Repository mapper
/// </summary>
public interface IDbEntityMapper<TFrom, TTo>
{
    /// <summary>
    /// Map entity
    /// </summary>
    /// <param name="entity">Entity to map</param>
    /// <returns>Mapped entity</returns>
    TTo Map(TFrom entity);
}
