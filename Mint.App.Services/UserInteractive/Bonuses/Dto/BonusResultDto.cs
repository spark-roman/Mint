namespace Mint.App.Services.UserInteractive.Bonuses.Dto;

/// <summary>
/// Result of bonus availability calculation.
/// </summary>
public record BonusResultDto
{
    /// <summary>
    /// Indicates whether the bonus operation was completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Indicates whether the bonus was not applied because it was already claimed before.
    /// Used to distinguish between "already claimed" and "not eligible" states.
    /// </summary>
    public bool AlreadyApplied { get; init; }

    /// <summary>
    /// Human-readable message describing the result of the operation.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// The total amount of bonus awarded (including streak bonus if applicable).
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Indicates whether a streak bonus was awarded in addition to the daily bonus.
    /// </summary>
    public bool IsStreakBonusApplied { get; init; }
}