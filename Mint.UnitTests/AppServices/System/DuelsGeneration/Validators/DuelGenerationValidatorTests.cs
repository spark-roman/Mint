using System.Collections.ObjectModel;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.App.Services.System.DuelsGeneration.Validators;

namespace Mint.UnitTests.AppServices.System.DuelsGeneration.Validators;

/// <summary>
/// Tests for <see cref="DuelGenerationValidator"/>
/// </summary>
public class DuelGenerationValidatorTests
{
    private readonly DuelGenerationValidator _validator = new();

    /// <summary>
    /// Verifies that null input throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void Validate_NullDuels_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => _validator.Validate(null!));
    }

    /// <summary>
    /// Verifies that empty collection returns invalid result.
    /// </summary>
    [Fact]
    public void Validate_EmptyCollection_ReturnsInvalid()
    {
        // Arrange
        var duels = new Collection<DuelGenerationDto>();

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("No duels to validate", result.Message);
    }

    /// <summary>
    /// Verifies that valid duel returns valid result.
    /// </summary>
    [Fact]
    public void Validate_ValidDuel_ReturnsValid()
    {
        // Arrange
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = "tech",
                DuelType = 1,
                Question = "Will AI replace programmers?",
                Description = "AI is advancing rapidly in code generation.",
                Options = ["Yes", "No"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.Message);
    }

    /// <summary>
    /// Verifies that multiple valid duels return valid result.
    /// </summary>
    [Fact]
    public void Validate_MultipleValidDuels_ReturnsValid()
    {
        // Arrange
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = "tech",
                DuelType = 1,
                Question = "Will AI replace programmers?",
                Description = "AI is advancing rapidly in code generation.",
                Options = ["Yes", "No"]
            },
            new DuelGenerationDto
            {
                CategoryCode = "crypto",
                DuelType = 2,
                Question = "Will Bitcoin hit $200k?",
                Description = "Institutional adoption is growing.",
                Options = ["Yes", "No", "Maybe"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifies that options count less than 2 returns invalid.
    /// </summary>
    [Fact]
    public void Validate_OptionCountLessThanTwo_ReturnsInvalid()
    {
        // Arrange
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = "tech",
                DuelType = 1,
                Question = "Test?",
                Description = "Test description",
                Options = ["Yes"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("must be 2-4", result.Message);
    }

    /// <summary>
    /// Verifies that options count greater than 4 returns invalid.
    /// </summary>
    [Fact]
    public void Validate_OptionCountGreaterThanFour_ReturnsInvalid()
    {
        // Arrange
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = "tech",
                DuelType = 1,
                Question = "Test?",
                Description = "Test description",
                Options = ["A", "B", "C", "D", "E"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("must be 2-4", result.Message);
    }

    /// <summary>
    /// Verifies that question length over 150 characters returns invalid.
    /// </summary>
    [Fact]
    public void Validate_QuestionTooLong_ReturnsInvalid()
    {
        // Arrange
        var longQuestion = new string('A', 151);
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = "tech",
                DuelType = 1,
                Question = longQuestion,
                Description = "Test description",
                Options = ["Yes", "No"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("too long", result.Message);
    }

    /// <summary>
    /// Verifies that description length over 500 characters returns invalid.
    /// </summary>
    [Fact]
    public void Validate_DescriptionTooLong_ReturnsInvalid()
    {
        // Arrange
        var longDescription = new string('B', 501);
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = "tech",
                DuelType = 1,
                Question = "Test?",
                Description = longDescription,
                Options = ["Yes", "No"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("too long", result.Message);
    }

    /// <summary>
    /// Verifies that empty category code returns invalid.
    /// </summary>
    [Fact]
    public void Validate_EmptyCategoryCode_ReturnsInvalid()
    {
        // Arrange
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = string.Empty,
                DuelType = 1,
                Question = "Test?",
                Description = "Test description",
                Options = ["Yes", "No"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("empty", result.Message);
    }

    /// <summary>
    /// Verifies that duplicate options return invalid.
    /// </summary>
    [Fact]
    public void Validate_DuplicateOptions_ReturnsInvalid()
    {
        // Arrange
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = "tech",
                DuelType = 1,
                Question = "Test?",
                Description = "Test description",
                Options = ["Yes", "Yes"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Duplicate", result.Message);
    }

    /// <summary>
    /// Verifies that invalid duel type returns invalid.
    /// </summary>
    [Fact]
    public void Validate_InvalidDuelType_ReturnsInvalid()
    {
        // Arrange
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = "tech",
                DuelType = 5,
                Question = "Test?",
                Description = "Test description",
                Options = ["Yes", "No"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Invalid duel_type", result.Message);
    }

    /// <summary>
    /// Verifies that duel type 1 (opinion) is valid.
    /// </summary>
    [Fact]
    public void Validate_DuelTypeOne_ReturnsValid()
    {
        // Arrange
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = "tech",
                DuelType = 1,
                Question = "Test?",
                Description = "Test description",
                Options = ["Yes", "No"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifies that duel type 2 (fact prediction) is valid.
    /// </summary>
    [Fact]
    public void Validate_DuelTypeTwo_ReturnsValid()
    {
        // Arrange
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = "tech",
                DuelType = 2,
                Question = "Test?",
                Description = "Test description",
                Options = ["Yes", "No"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifies that 4 options is valid.
    /// </summary>
    [Fact]
    public void Validate_FourOptions_ReturnsValid()
    {
        // Arrange
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = "games",
                DuelType = 1,
                Question = "Best game of 2026?",
                Description = "GTA VI, TES VI, Cyberpunk 2 and Half-Life 3 released.",
                Options = ["GTA VI", "TES VI", "Cyberpunk 2", "Half-Life 3"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.True(result.IsValid);
    }

    /// <summary>
    /// Verifies that first invalid duel in collection stops validation.
    /// </summary>
    [Fact]
    public void Validate_FirstInvalidDuel_StopsAtFirst()
    {
        // Arrange
        var duels = new Collection<DuelGenerationDto>
        {
            new DuelGenerationDto
            {
                CategoryCode = "tech",
                DuelType = 1,
                Question = new string('A', 151),
                Description = "Test description",
                Options = ["Yes", "No"]
            },
            new DuelGenerationDto
            {
                CategoryCode = "crypto",
                DuelType = 2,
                Question = "Test?",
                Description = new string('B', 501),
                Options = ["Yes", "No"]
            }
        };

        // Act
        var result = _validator.Validate(duels);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("too long", result.Message);
        Assert.Contains("151", result.Message);
    }
}
