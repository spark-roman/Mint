using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.News.RSS.Dto;

namespace Mint.Database.Entities.News.RSS.Mappers;

/// <summary>
/// Mapper from RssSourceCreateDto to RssSourceEntity.
/// </summary>
public sealed class DbRssSourceCreateMapper : IDbEntityMapper<RssSourceCreateDto, RssSourceEntity>
{
    /// <inheritdoc/>
    public RssSourceEntity Map(RssSourceCreateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new RssSourceEntity
        {
            Name = entity.Name,
            Url = entity.Url,
            CategoryCode = entity.CategoryCode,
            Language = entity.Language,
            Priority = entity.Priority,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
}