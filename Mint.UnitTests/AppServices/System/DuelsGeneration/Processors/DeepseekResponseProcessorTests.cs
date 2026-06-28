using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.App.Services.System.DuelsGeneration.Processors;
using Mint.App.Services.System.DuelsGeneration.Validators;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Moq;
using OptionDto = Mint.App.Services.System.DuelsGeneration.Dto.OptionGenerationDto;

namespace Mint.UnitTests.AppServices.System.DuelsGeneration.Processors;

/// <summary>
/// Tests for <see cref="DeepseekResponseProcessor"/>
/// </summary>
public class DeepseekResponseProcessorTests
{
    private readonly Mock<IDtoMapper<DuelGenerationDto, DuelCreateDto>> _mockMapper;
    private readonly Mock<IDuelGenerationValidator> _mockValidator;
    private readonly DeepseekResponseProcessor _processor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeepseekResponseProcessorTests"/> class.
    /// </summary>
    public DeepseekResponseProcessorTests()
    {
        _mockMapper = new Mock<IDtoMapper<DuelGenerationDto, DuelCreateDto>>();
        _mockValidator = new Mock<IDuelGenerationValidator>();
        _processor = new DeepseekResponseProcessor(_mockMapper.Object, _mockValidator.Object);
    }

    /// <summary>
    /// Verifies that Process correctly converts a valid AI response to DuelCreateDto list.
    /// </summary>
    [Fact]
    public async Task Process_ValidResponse_ReturnsDuels()
    {
        // Arrange
        var duelDto = new DuelGenerationDto
        {
            CategoryCode = "tech",
            DuelType = 1,
            Question = "Will AI replace programmers?",
            Description = "AI is advancing rapidly in code generation.",
            Options = new Collection<OptionDto>
            {
                new() { Code = "a", Text = "Yes" },
                new() { Code = "b", Text = "No" }
            }
        };

        var duelsCollection = new Collection<DuelGenerationDto> { duelDto };
        var responseContent = JsonSerializer.Serialize(duelsCollection);

        var expectedDuel = new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = Mint.Common.Contracts.UserInteractive.DuelType.OpinionMatch,
            Question = "Will AI replace programmers?",
            Description = "AI is advancing rapidly in code generation.",
            ExpiresAt = DateTimeOffset.MaxValue,
            Options = new Collection<DuelOptionCreateDto>
            {
                new() { OptionCode = "a", OptionText = "Yes" },
                new() { OptionCode = "b", OptionText = "No" }
            }
        };

        _mockValidator.Setup(v => v.Validate(It.IsAny<Collection<DuelGenerationDto>? >())).Returns(new GenerationValidationResult { IsValid = true, Message = null });
        _mockMapper.Setup(m => m.Map(It.IsAny<DuelGenerationDto>(), It.IsAny<int>(), It.IsAny<int>())).Returns(expectedDuel);

        // Act
        var result = await _processor.Process(responseContent, categoryId: 1, daysToLive: 7);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        var first = result.First();
        first.Question.Should().Be("Will AI replace programmers?");
        first.Description.Should().Be("AI is advancing rapidly in code generation.");
        first.Options.Should().HaveCount(2);
        _mockValidator.Verify(v => v.Validate(It.IsAny<Collection<DuelGenerationDto>? >()), Times.Once);
        _mockMapper.Verify(m => m.Map(It.IsAny<DuelGenerationDto>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }

    /// <summary>
    /// Verifies that Process correctly handles multiple duels in response.
    /// </summary>
    [Fact]
    public async Task Process_MultipleDuels_ReturnsAllDuels()
    {
        // Arrange
        var duel1 = new DuelGenerationDto
        {
            CategoryCode = "tech",
            DuelType = 1,
            Question = "Will AI replace programmers?",
            Description = "AI is advancing.",
            Options = new Collection<OptionDto>
            {
                new() { Code = "a", Text = "Yes" },
                new() { Code = "b", Text = "No" }
            }
        };

        var duel2 = new DuelGenerationDto
        {
            CategoryCode = "crypto",
            DuelType = 2,
            Question = "Will Bitcoin hit $200k?",
            Description = "Institutional adoption is growing.",
            Options = new Collection<OptionDto>
            {
                new() { Code = "a", Text = "Yes" },
                new() { Code = "b", Text = "No" }
            }
        };

        var duelsCollection = new Collection<DuelGenerationDto> { duel1, duel2 };
        var responseContent = JsonSerializer.Serialize(duelsCollection);

        _mockValidator.Setup(v => v.Validate(It.IsAny<Collection<DuelGenerationDto>? >()))
            .Returns(new GenerationValidationResult { IsValid = true, Message = null });

        _mockMapper.Setup(m => m.Map(It.IsAny<DuelGenerationDto>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((DuelGenerationDto dto, object[] args) =>
            {
                return dto.Question switch
                {
                    "Will AI replace programmers?" => new DuelCreateDto
                    {
                        CategoryId = 1,
                        Question = "Will AI replace programmers?",
                        Description = "AI is advancing.",
                        ExpiresAt = DateTimeOffset.MaxValue,
                        Options = [],
                        DuelType = Mint.Common.Contracts.UserInteractive.DuelType.OpinionMatch
                    },
                    "Will Bitcoin hit $200k?" => new DuelCreateDto
                    {
                        CategoryId = 1,
                        Question = "Will Bitcoin hit $200k?",
                        Description = "Institutional adoption is growing.",
                        ExpiresAt = DateTimeOffset.MaxValue,
                        Options = [],
                        DuelType = Mint.Common.Contracts.UserInteractive.DuelType.FactPrediction
                    },
                    _ => new DuelCreateDto
                    {
                        CategoryId = 1,
                        Question = dto.Question,
                        Description = dto.Description,
                        ExpiresAt = DateTimeOffset.MaxValue,
                        Options = [],
                        DuelType = Mint.Common.Contracts.UserInteractive.DuelType.None
                    }
                };
            });

        // Act
        var result = await _processor.Process(responseContent, categoryId: 1, daysToLive: 7);

        // Assert
        result.Should().HaveCount(2);
        result.First().Question.Should().Be("Will AI replace programmers?");
        result.ElementAt(1).Question.Should().Be("Will Bitcoin hit $200k?");
        _mockMapper.Verify(m => m.Map(It.IsAny<DuelGenerationDto>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
    }

    /// <summary>
    /// Verifies that Process throws when validation fails.
    /// </summary>
    [Fact]
    public async Task Process_InvalidResponse_ThrowsInvalidOperationException()
    {
        // Arrange
        var duelDto = new DuelGenerationDto
        {
            CategoryCode = "tech",
            DuelType = 1,
            Question = "Test?",
            Description = "Test",
            Options = new Collection<OptionDto>
            {
                new() { Code = "a", Text = "Yes" }
            }
        };

        var duelsCollection = new Collection<DuelGenerationDto> { duelDto };
        var responseContent = JsonSerializer.Serialize(duelsCollection);

        _mockValidator.Setup(v => v.Validate(It.IsAny<Collection<DuelGenerationDto>? >()))
            .Returns(new GenerationValidationResult { IsValid = false, Message = "Options count must be 2-4" });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _processor.Process(responseContent, 1, 7));
        _mockMapper.Verify(m => m.Map(It.IsAny<DuelGenerationDto>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    /// <summary>
    /// Verifies that Process throws on empty collection.
    /// </summary>
    [Fact]
    public async Task Process_EmptyCollection_ThrowsInvalidOperationException()
    {
        // Arrange
        var duelsCollection = new Collection<DuelGenerationDto>();
        var responseContent = JsonSerializer.Serialize(duelsCollection);

        _mockValidator.Setup(v => v.Validate(It.IsAny<Collection<DuelGenerationDto>? >()))
            .Returns(new GenerationValidationResult { IsValid = false, Message = "No duels to validate" });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _processor.Process(responseContent, 1, 7));
    }

    /// <summary>
    /// Verifies that Process throws on invalid JSON.
    /// </summary>
    [Fact]
    public async Task Process_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        var invalidJson = "not valid json";

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(async () => await _processor.Process(invalidJson, 1, 7));
        _mockValidator.Verify(v => v.Validate(It.IsAny<Collection<DuelGenerationDto>? >()), Times.Never);
    }

    /// <summary>
    /// Verifies that Process throws on null input.
    /// </summary>
    [Fact]
    public async Task Process_NullInput_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _processor.Process(null!, 1, 7));
    }

    /// <summary>
    /// Verifies that Process throws when AI returns null/empty content.
    /// </summary>
    [Fact]
    public async Task Process_EmptyContent_ThrowsInvalidOperationException()
    {
        // Arrange
        var emptyContent = string.Empty;

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(async () => await _processor.Process(emptyContent, 1, 7));
    }

    /// <summary>
    /// Verifies that Process correctly maps duel type 1 to OpinionMatch.
    /// </summary>
    [Fact]
    public async Task Process_DuelType1_MapsToOpinionMatch()
    {
        // Arrange
        var duelDto = new DuelGenerationDto
        {
            CategoryCode = "tech",
            DuelType = 1,
            Question = "Test?",
            Description = "Test",
            Options = new Collection<OptionDto>
            {
                new() { Code = "a", Text = "Yes" },
                new() { Code = "b", Text = "No" }
            }
        };

        var duelsCollection = new Collection<DuelGenerationDto> { duelDto };
        var responseContent = JsonSerializer.Serialize(duelsCollection);

        _mockValidator.Setup(v => v.Validate(It.IsAny<Collection<DuelGenerationDto>? >()))
            .Returns(new GenerationValidationResult { IsValid = true, Message = null });

        _mockMapper.Setup(m => m.Map(It.IsAny<DuelGenerationDto>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(new DuelCreateDto
            {
                CategoryId = 1,
                Question = "Test?",
                Description = "Test",
                ExpiresAt = DateTimeOffset.MaxValue,
                Options = [],
                DuelType = Mint.Common.Contracts.UserInteractive.DuelType.OpinionMatch
            });

        // Act
        var result = await _processor.Process(responseContent, categoryId: 1, daysToLive: 7);

        // Assert
        result.First().DuelType.Should().Be(Mint.Common.Contracts.UserInteractive.DuelType.OpinionMatch);
    }

    /// <summary>
    /// Verifies that Process correctly maps duel type 2 to FactPrediction.
    /// </summary>
    [Fact]
    public async Task Process_DuelType2_MapsToFactPrediction()
    {
        // Arrange
        var duelDto = new DuelGenerationDto
        {
            CategoryCode = "crypto",
            DuelType = 2,
            Question = "Test?",
            Description = "Test",
            Options = new Collection<OptionDto>
            {
                new() { Code = "a", Text = "Yes" },
                new() { Code = "b", Text = "No" }
            }
        };

        var duelsCollection = new Collection<DuelGenerationDto> { duelDto };
        var responseContent = JsonSerializer.Serialize(duelsCollection);

        _mockValidator.Setup(v => v.Validate(It.IsAny<Collection<DuelGenerationDto>? >()))
            .Returns(new GenerationValidationResult { IsValid = true, Message = null });

        _mockMapper.Setup(m => m.Map(It.IsAny<DuelGenerationDto>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(new DuelCreateDto
            {
                CategoryId = 1,
                Question = "Test?",
                Description = "Test",
                ExpiresAt = DateTimeOffset.MaxValue,
                Options = [],
                DuelType = Mint.Common.Contracts.UserInteractive.DuelType.FactPrediction
            });

        // Act
        var result = await _processor.Process(responseContent, categoryId: 1, daysToLive: 7);

        // Assert
        result.First().DuelType.Should().Be(Mint.Common.Contracts.UserInteractive.DuelType.FactPrediction);
    }
}
