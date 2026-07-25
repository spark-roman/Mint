using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.News.Dto;

namespace Mint.Database.Entities.News.Mappers;

/// <summary>
/// Mapper from NewsEntity to NewsDto.
/// </summary>
public sealed class DbNewsMapper : IDbEntityMapper<NewsEntity, NewsDto>
{
    /// <inheritdoc/>
    public NewsDto Map(NewsEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new NewsDto
        {
            Id = entity.Id,
            RssSourceId = entity.RssSourceId,
            Title = entity.Title,
            Link = entity.Link,
            Description = entity.Description,
            Content = entity.Content,
            Author = entity.Author,
            CategoryCode = entity.CategoryCode,
            PublishedAt = entity.PublishedAt,
            IsProcessed = entity.IsProcessed,
            ProcessedAt = entity.ProcessedAt,
            CreatedAt = entity.CreatedAt
        };
    }
}
