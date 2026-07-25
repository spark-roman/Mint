using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.News.RSS.Dto;

namespace Mint.Database.Entities.News.RSS.Mappers;

/// <summary>
/// Mapper from RssSourceEntity to RssSourceDto.
/// </summary>
public sealed class DbRssSourceMapper : IDbEntityMapper<RssSourceEntity, RssSourceDto>
{
    /// <inheritdoc/>
    public RssSourceDto Map(RssSourceEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new RssSourceDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Url = entity.Url,
            CategoryCode = entity.CategoryCode,
            IsActive = entity.IsActive,
            Priority = entity.Priority,
            Language = entity.Language,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
