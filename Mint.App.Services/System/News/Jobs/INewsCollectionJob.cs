namespace Mint.App.Services.System.News.Jobs;

/// <summary>
/// Interface for news collection job.
/// </summary>
public interface INewsCollectionJob
{
    /// <summary>
    /// Executes the news collection job.
    /// </summary>
    Task ExecuteAsync();
}
