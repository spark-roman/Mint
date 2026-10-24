namespace Mint.App.Services.Infrastructure.DI.System.Referral;

/// <summary>
/// Referral service settings
/// </summary>
public record ReferralSettings
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "Referral";

    /// <summary>
    /// Salt for Hashids
    /// </summary>
    public string Salt { get; init; } = "default-test-salt";

    /// <summary>
    /// Minimum referral code length
    /// </summary>
    public int MinLength { get; init; } = 8;
}
