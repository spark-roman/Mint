using System.Collections.ObjectModel;
using System.Net;
using Mint.App.Services.System.DuelsGeneration;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;
using Mint.UnitTests.AppServices.System.Fixtures.Seeding;
using Microsoft.Extensions.DependencyInjection;

namespace Mint.UnitTests.AppServices.System.DuelsGeneration;

/// <summary>
/// Tests for <see cref="DuelGenerationService"/> using EF Core and DI registrations.
/// </summary>
public class DuelGenerationServiceTests : IClassFixture<DuelGenerationServiceFixture>, IDisposable
{
    private readonly DuelGenerationServiceFixture _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelGenerationServiceTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public DuelGenerationServiceTests(DuelGenerationServiceFixture fixture)
    {
        _fixture = fixture;
    }

    #region GenerateDuelsAsync

    /// <summary>
    /// Verifies that GenerateDuelsAsync generates duels for all active categories.
    /// </summary>
    [Fact]
    public async Task GenerateDuelsAsync_GeneratesDuels_ReturnsDuels()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var service = _currentScope.ServiceProvider.GetRequiredService<IDuelGenerationService>();

        var duelDto = new DuelGenerationDto
        {
            CategoryCode = "tech",
            DuelType = 1,
            Question = "Will AI replace programmers?",
            Description = "AI is advancing rapidly in code generation.",
            Options = new Collection<OptionGenerationDto>
            {
                new() { Code = "a", Text = "Yes" },
                new() { Code = "b", Text = "No" }
            }
        };

        var jsonResponse = DuelsSeeder.CreateValidApiResponse(new Collection<DuelGenerationDto> { duelDto });
        _fixture.SetHttpResponse(jsonResponse);

        // Act
        var result = await service.GenerateDuelsAsync(1, 3, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);
    }

    /// <summary>
    /// Verifies that GenerateDuelsAsync works with seeded prompt.
    /// </summary>
    [Fact]
    public async Task GenerateDuelsAsync_WithSeededPrompt_ReturnsDuels()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var service = _currentScope.ServiceProvider.GetRequiredService<IDuelGenerationService>();

        var duelDto = new DuelGenerationDto
        {
            CategoryCode = "tech",
            DuelType = 1,
            Question = "Will AI replace programmers?",
            Description = "AI is advancing rapidly in code generation.",
            Options = new Collection<OptionGenerationDto>
            {
                new() { Code = "a", Text = "Yes" },
                new() { Code = "b", Text = "No" }
            }
        };

        var jsonResponse = DuelsSeeder.CreateValidApiResponse(new Collection<DuelGenerationDto> { duelDto });
        _fixture.SetHttpResponse(jsonResponse);

        // Act - repository ignores promptId and returns first prompt
        var result = await service.GenerateDuelsAsync(1, 3, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);
    }

    #endregion

    #region GenerateDuelsForAllActiveCategoriesAsync

    /// <summary>
    /// Verifies that GenerateDuelsForAllActiveCategoriesAsync generates duels for all prompts.
    /// </summary>
    [Fact]
    public async Task GenerateDuelsForAllActiveCategoriesAsync_GeneratesDuels_ReturnsDuels()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var service = _currentScope.ServiceProvider.GetRequiredService<IDuelGenerationService>();

        var duelDto = new DuelGenerationDto
        {
            CategoryCode = "tech",
            DuelType = 1,
            Question = "Will AI replace programmers?",
            Description = "AI is advancing.",
            Options = new Collection<OptionGenerationDto>
            {
                new() { Code = "a", Text = "Yes" },
                new() { Code = "b", Text = "No" }
            }
        };

        var jsonResponse = DuelsSeeder.CreateValidApiResponse(new Collection<DuelGenerationDto> { duelDto });
        _fixture.SetHttpResponse(jsonResponse);

        // Act
        var result = await service.GenerateDuelsForAllActiveCategoriesAsync(3, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);
    }

    /// <summary>
    /// Verifies that GenerateDuelsForAllActiveCategoriesAsync returns results with seeded data.
    /// </summary>
    [Fact]
    public async Task GenerateDuelsForAllActiveCategoriesAsync_WithSeededData_ReturnsDuels()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var service = _currentScope.ServiceProvider.GetRequiredService<IDuelGenerationService>();

        var duelDto = new DuelGenerationDto
        {
            CategoryCode = "tech",
            DuelType = 1,
            Question = "Will AI replace programmers?",
            Description = "AI is advancing.",
            Options = new Collection<OptionGenerationDto>
            {
                new() { Code = "a", Text = "Yes" },
                new() { Code = "b", Text = "No" }
            }
        };

        var jsonResponse = DuelsSeeder.CreateValidApiResponse(new Collection<DuelGenerationDto> { duelDto });
        _fixture.SetHttpResponse(jsonResponse);

        // Act
        var result = await service.GenerateDuelsForAllActiveCategoriesAsync(3, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region HTTP Error Handling

    /// <summary>
    /// Verifies that GenerateDuelsAsync throws on HTTP error from DeepSeek API.
    /// </summary>
    [Fact]
    public async Task GenerateDuelsAsync_ApiError_ThrowsException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var service = _currentScope.ServiceProvider.GetRequiredService<IDuelGenerationService>();

        _fixture.SetHttpErrorResponse(HttpStatusCode.BadGateway, "API unavailable");

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => service.GenerateDuelsAsync(1, 3, CancellationToken.None));
    }

    #endregion

    #region Validation

    /// <summary>
    /// Verifies that GenerateDuelsAsync works with valid data and real validator.
    /// </summary>
    [Fact]
    public async Task GenerateDuelsAsync_WithValidData_PassesValidation()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var service = _currentScope.ServiceProvider.GetRequiredService<IDuelGenerationService>();

        var duelDto = new DuelGenerationDto
        {
            CategoryCode = "tech",
            DuelType = 1,
            Question = "Test?",
            Description = "Test",
            Options = new Collection<OptionGenerationDto>
            {
                new() { Code = "a", Text = "Yes" },
                new() { Code = "b", Text = "No" }
            }
        };

        var jsonResponse = DuelsSeeder.CreateValidApiResponse(new Collection<DuelGenerationDto> { duelDto });
        _fixture.SetHttpResponse(jsonResponse);

        // Act
        var result = await service.GenerateDuelsAsync(1, 3, CancellationToken.None);

        // Assert - validation passes by default with real validator
        Assert.NotEmpty(result);
    }

    /// <summary>
    /// Verifies that GenerateDuelsAsync throws when AI returns empty duels.
    /// </summary>
    [Fact]
    public async Task GenerateDuelsAsync_EmptyDuels_ThrowsException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var service = _currentScope.ServiceProvider.GetRequiredService<IDuelGenerationService>();

        // Empty choices response
        _fixture.SetHttpResponse("{\"choices\":[{\"message\":{\"content\":\"[]\"}}]}");

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => service.GenerateDuelsAsync(1, 3, CancellationToken.None));
    }

    #endregion

    private bool _disposed;

    /// <inheritdoc />
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _currentScope?.Dispose();
        }

        _disposed = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
