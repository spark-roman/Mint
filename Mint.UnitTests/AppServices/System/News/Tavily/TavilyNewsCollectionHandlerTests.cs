using System.Collections.ObjectModel;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mint.App.Services.System.News.Dto;
using Mint.App.Services.System.News.Handlers;
using Mint.App.Services.System.News.Tavily;
using Mint.Database.Entities.News.Dto;
using Mint.Database.Entities.News.Repositories;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;
using Moq;

namespace Mint.UnitTests.AppServices.System.News.Tavily;

/// <summary>
/// Tests for <see cref="TavilyNewsCollectionHandler"/>
/// </summary>
public class TavilyNewsCollectionHandlerTests
{
    private readonly Mock<ITavilyClient> _mockTavilyClient;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<INewsRepository> _mockNewsRepository;
    private readonly Mock<IOptions<TavilySettings>> _mockSettings;
    private readonly Mock<TimeProvider> _mockTimeProvider;
    private readonly Mock<ILogger<TavilyNewsCollectionHandler>> _mockLogger;
    private readonly TavilyNewsCollectionHandler _handler;

    private readonly DateTimeOffset _fixedNow = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);

    public TavilyNewsCollectionHandlerTests()
    {
        _mockTavilyClient = new Mock<ITavilyClient>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockNewsRepository = new Mock<INewsRepository>();
        _mockSettings = new Mock<IOptions<TavilySettings>>();
        _mockSettings.Setup(s => s.Value).Returns(new TavilySettings
        {
            ApiKey = "test-key",
            DaysBack = 30,
            ExcludeDomains = new Collection<string> { "example.com" }
        });
        _mockTimeProvider = new Mock<TimeProvider>();
        _mockTimeProvider.Setup(t => t.GetUtcNow()).Returns(_fixedNow);
        _mockLogger = new Mock<ILogger<TavilyNewsCollectionHandler>>();

        _handler = new TavilyNewsCollectionHandler(
            _mockTavilyClient.Object,
            _mockCategoryRepository.Object,
            _mockNewsRepository.Object,
            _mockSettings.Object,
            _mockTimeProvider.Object,
            _mockLogger.Object);
    }

    #region CollectAsync - Success

    [Fact]
    public async Task CollectAsync_NoCategories_ReturnsEmptyResult()
    {
        // Arrange
        _mockCategoryRepository.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CategoryDto>());

        // Act
        var result = await _handler.CollectAsync(CancellationToken.None);

        // Assert
        result.TotalFetched.Should().Be(0);
        result.NewSaved.Should().Be(0);
        result.SkippedDuplicates.Should().Be(0);
        result.FailedSources.Should().Be(0);
        result.Errors.Should().BeEmpty();

        _mockTavilyClient.Verify(c => c.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockNewsRepository.Verify(r => r.CreateManyAsync(It.IsAny<Collection<NewsCreateDto>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CollectAsync_OneCategory_ReturnsCollectedNews()
    {
        // Arrange
        var categories = new List<CategoryDto>
        {
            new() { Id = 1, Name = "AI", Code = "ai", SearchKeywords = "ai, chatgpt", IsActiveForAI = true }
        };
        _mockCategoryRepository.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var newsItems = new List<NewsCreateDto>
        {
            new()
            {
                Title = "AI News",
                Link = "https://example.com/ai",
                Content = "AI content",
                PublishedAt = _fixedNow,
                CategoryCode = "ai",
                RelevanceScore = 0.9
            }
        };
        _mockTavilyClient.Setup(c => c.SearchAsync("ai chatgpt", "ai", CancellationToken.None))
            .ReturnsAsync(newsItems);

        _mockNewsRepository.Setup(r => r.GetLinksSinceAsync(It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>());

        _mockNewsRepository.Setup(r => r.CreateManyAsync(It.IsAny<Collection<NewsCreateDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NewsDto>());

        // Act
        var result = await _handler.CollectAsync(CancellationToken.None);

        // Assert
        result.TotalFetched.Should().Be(1);
        result.NewSaved.Should().Be(1);
        result.SkippedDuplicates.Should().Be(0);
        result.FailedSources.Should().Be(0);
        result.Errors.Should().BeEmpty();

        _mockTavilyClient.Verify(c => c.SearchAsync("ai chatgpt", "ai", CancellationToken.None), Times.Once);
        _mockNewsRepository.Verify(r => r.CreateManyAsync(It.Is<Collection<NewsCreateDto>>(c => c.Count == 1), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CollectAsync_MultipleCategories_CallsAll()
    {
        // Arrange
        var categories = new List<CategoryDto>
        {
            new() { Id = 1, Name = "AI", Code = "ai", SearchKeywords = "ai", IsActiveForAI = true },
            new() { Id = 2, Name = "Crypto", Code = "crypto", SearchKeywords = "bitcoin", IsActiveForAI = true }
        };
        _mockCategoryRepository.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        _mockTavilyClient.Setup(c => c.SearchAsync("ai", "ai", CancellationToken.None))
            .ReturnsAsync(new List<NewsCreateDto>
            {
                new() { Title = "AI", Link = "https://example.com/ai", Content = "AI", PublishedAt = _fixedNow, CategoryCode = "ai" }
            });
        _mockTavilyClient.Setup(c => c.SearchAsync("bitcoin", "crypto", CancellationToken.None))
            .ReturnsAsync(new List<NewsCreateDto>
            {
                new() { Title = "BTC", Link = "https://example.com/btc", Content = "BTC", PublishedAt = _fixedNow, CategoryCode = "crypto" }
            });

        _mockNewsRepository.Setup(r => r.GetLinksSinceAsync(It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>());

        _mockNewsRepository.Setup(r => r.CreateManyAsync(It.IsAny<Collection<NewsCreateDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NewsDto>());

        // Act
        var result = await _handler.CollectAsync(CancellationToken.None);

        // Assert
        result.TotalFetched.Should().Be(2);
        result.NewSaved.Should().Be(2);
        _mockTavilyClient.Verify(c => c.SearchAsync("ai", "ai", CancellationToken.None), Times.Once);
        _mockTavilyClient.Verify(c => c.SearchAsync("bitcoin", "crypto", CancellationToken.None), Times.Once);
    }

    #endregion

    #region CollectAsync - Deduplication

    [Fact]
    public async Task CollectAsync_DuplicateLinks_SkipsDuplicates()
    {
        // Arrange
        var categories = new List<CategoryDto>
        {
            new() { Id = 1, Name = "AI", Code = "ai", SearchKeywords = "ai", IsActiveForAI = true }
        };
        _mockCategoryRepository.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var newsItems = new List<NewsCreateDto>
        {
            new() { Title = "News 1", Link = "https://example.com/1", Content = "1", PublishedAt = _fixedNow, CategoryCode = "ai" },
            new() { Title = "News 2", Link = "https://example.com/2", Content = "2", PublishedAt = _fixedNow, CategoryCode = "ai" },
            new() { Title = "News 3", Link = "https://example.com/3", Content = "3", PublishedAt = _fixedNow, CategoryCode = "ai" }
        };
        _mockTavilyClient.Setup(c => c.SearchAsync("ai", "ai", CancellationToken.None))
            .ReturnsAsync(newsItems);

        // Simulate 1 existing link
        var existingLinks = new HashSet<string> { "https://example.com/1" };
        _mockNewsRepository.Setup(r => r.GetLinksSinceAsync(It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingLinks);

        _mockNewsRepository.Setup(r => r.CreateManyAsync(It.IsAny<Collection<NewsCreateDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NewsDto>());

        // Act
        var result = await _handler.CollectAsync(CancellationToken.None);

        // Assert
        result.TotalFetched.Should().Be(3);
        result.SkippedDuplicates.Should().Be(1);
        result.NewSaved.Should().Be(2);
    }

    [Fact]
    public async Task CollectAsync_AllDuplicates_NewSavedIsZero()
    {
        // Arrange
        var categories = new List<CategoryDto>
        {
            new() { Id = 1, Name = "AI", Code = "ai", SearchKeywords = "ai", IsActiveForAI = true }
        };
        _mockCategoryRepository.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var newsItems = new List<NewsCreateDto>
        {
            new() { Title = "News", Link = "https://example.com/1", Content = "1", PublishedAt = _fixedNow, CategoryCode = "ai" }
        };
        _mockTavilyClient.Setup(c => c.SearchAsync("ai", "ai", CancellationToken.None))
            .ReturnsAsync(newsItems);

        _mockNewsRepository.Setup(r => r.GetLinksSinceAsync(It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string> { "https://example.com/1" });

        // Act
        var result = await _handler.CollectAsync(CancellationToken.None);

        // Assert
        result.TotalFetched.Should().Be(1);
        result.SkippedDuplicates.Should().Be(1);
        result.NewSaved.Should().Be(0);
        _mockNewsRepository.Verify(r => r.CreateManyAsync(It.IsAny<Collection<NewsCreateDto>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CollectAsync_CrossCategoryDuplicates_Removed()
    {
        // Arrange
        var categories = new List<CategoryDto>
        {
            new() { Id = 1, Name = "AI", Code = "ai", SearchKeywords = "ai", IsActiveForAI = true },
            new() { Id = 2, Name = "Tech", Code = "tech", SearchKeywords = "tech", IsActiveForAI = true }
        };
        _mockCategoryRepository.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        _mockTavilyClient.Setup(c => c.SearchAsync("ai", "ai", CancellationToken.None))
            .ReturnsAsync(new List<NewsCreateDto>
            {
                new() { Title = "Same", Link = "https://example.com/same", Content = "Same", PublishedAt = _fixedNow, CategoryCode = "ai" }
            });
        _mockTavilyClient.Setup(c => c.SearchAsync("tech", "tech", CancellationToken.None))
            .ReturnsAsync(new List<NewsCreateDto>
            {
                new() { Title = "Same", Link = "https://example.com/same", Content = "Same", PublishedAt = _fixedNow, CategoryCode = "tech" }
            });

        _mockNewsRepository.Setup(r => r.GetLinksSinceAsync(It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>());

        _mockNewsRepository.Setup(r => r.CreateManyAsync(It.IsAny<Collection<NewsCreateDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NewsDto>());

        // Act
        var result = await _handler.CollectAsync(CancellationToken.None);

        // Assert
        result.TotalFetched.Should().Be(2);
        result.NewSaved.Should().Be(1); // deduplicated across categories
    }

    #endregion

    #region CollectAsync - Error Handling

    [Fact]
    public async Task CollectAsync_CategoryThrows_ErrorRecorded()
    {
        // Arrange
        var categories = new List<CategoryDto>
        {
            new() { Id = 1, Name = "AI", Code = "ai", SearchKeywords = "ai", IsActiveForAI = true },
            new() { Id = 2, Name = "Crypto", Code = "crypto", SearchKeywords = "btc", IsActiveForAI = true }
        };
        _mockCategoryRepository.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        _mockTavilyClient.Setup(c => c.SearchAsync("ai", "ai", CancellationToken.None))
            .ReturnsAsync(new List<NewsCreateDto>
            {
                new() { Title = "AI", Link = "https://example.com/ai", Content = "AI", PublishedAt = _fixedNow, CategoryCode = "ai" }
            });
        _mockTavilyClient.Setup(c => c.SearchAsync("btc", "crypto", CancellationToken.None))
            .Throws(new InvalidOperationException("API rate limited"));

        _mockNewsRepository.Setup(r => r.GetLinksSinceAsync(It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>());

        _mockNewsRepository.Setup(r => r.CreateManyAsync(It.IsAny<Collection<NewsCreateDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NewsDto>());

        // Act
        var result = await _handler.CollectAsync(CancellationToken.None);

        // Assert
        result.TotalFetched.Should().Be(1);
        result.FailedSources.Should().Be(1);
        result.NewSaved.Should().Be(1);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().Contain("crypto");
        result.Errors[0].Should().Contain("rate limited");
    }

    [Fact]
    public async Task CollectAsync_EmptyKeywords_CategoryThrows()
    {
        // Arrange
        var categories = new List<CategoryDto>
        {
            new() { Id = 1, Name = "AI", Code = "ai", SearchKeywords = null, IsActiveForAI = true }
        };
        _mockCategoryRepository.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        _mockNewsRepository.Setup(r => r.GetLinksSinceAsync(It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>());

        // Act
        var result = await _handler.CollectAsync(CancellationToken.None);

        // Assert
        result.FailedSources.Should().Be(1);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().Contain("ai");
        result.Errors[0].Should().Contain("No search keywords");
    }

    #endregion

    #region CollectAsync - Settings

    [Fact]
    public async Task CollectAsync_UsesDaysBackForCutoffDate()
    {
        // Arrange
        _mockSettings.Setup(s => s.Value).Returns(new TavilySettings
        {
            ApiKey = "test-key",
            DaysBack = 14,
            ExcludeDomains = new Collection<string> { "example.com" }
        });
        _mockCategoryRepository.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CategoryDto>());

        // Act
        await _handler.CollectAsync(CancellationToken.None);

        // Assert — cutoff should be 14 days before fixed now
        var expectedCutoff = _fixedNow.AddDays(-14);
        _mockNewsRepository.Verify(
            r => r.GetLinksSinceAsync(It.Is<DateTimeOffset>(d => d == expectedCutoff), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion
}
