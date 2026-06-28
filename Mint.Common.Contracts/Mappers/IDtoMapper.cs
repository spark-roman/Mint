namespace Mint.Common.Contracts.Mappers;

/// <summary>
/// Dto mapper
/// </summary>
/// <typeparam name="TDtoFrom">Dto to map</typeparam>
/// <typeparam name="TDtoTo">Mapped dto type</typeparam>
public interface IDtoMapper<TDtoFrom, TDtoTo>
{
    /// <summary>
    /// Map dto
    /// </summary>
    /// <param name="dto">Dto to map</param>
    /// <param name="args">Additional arguments for mapping</param>
    /// <returns>Mapped dto</returns>
    TDtoTo Map(TDtoFrom dto, params object[] args);
}
