using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.News.Dto;

namespace Mint.Database.Entities.News.Mappers;

/// <summary>
/// Mapper from NewsCreateDto to NewsEntity.
/// </summary>
public sealed class DbNewsCreateMapper : IDbEntityMapper<NewsCreateDto, NewsEntity>
{
    /// <inheritdoc/>
    public NewsEntity Map(NewsCreateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new NewsEntity
        {
            RssSourceId = entity.RssSourceId,
            Title = entity.Title,
            Link = entity.Link,
            Description = entity.Description,
            Content = entity.Content,
            Author = entity.Author,
            CategoryCode = entity.CategoryCode,
            PublishedAt = entity.PublishedAt,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
