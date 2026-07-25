using System.Collections.ObjectModel;

namespace Mint.App.Services.System.News.Dto;

/// <summary>
/// Result of news collection operation.
/// </summary>
public sealed record NewsCollectionResult
{
    /// <summary>
    /// Total number of news fetched.
    /// </summary>
    public required int TotalFetched { get; set; }

    /// <summary>
    /// Number of new news saved.
    /// </summary>
    public required int NewSaved { get; set; }

    /// <summary>
    /// Number of news skipped due to duplicate.
    /// </summary>
    public required int SkippedDuplicates { get; set; }
    
    /// <summary>
    /// Number of failed sources.
    /// </summary>
    public required int FailedSources { get; set; }

    /// <summary>
    /// Error messages.
    /// </summary>
    public Collection<string> Errors { get; init; } = [];
}
