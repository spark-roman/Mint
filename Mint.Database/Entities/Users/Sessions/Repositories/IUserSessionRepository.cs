using Mint.Database.Entities.Users.Sessions.Dto;

namespace Mint.Database.Entities.Users.Sessions.Repositories;

/// <summary>
/// Provides data access operations for user sessions.
/// </summary>
public interface IUserSessionRepository
{
    /// <summary>
    /// Retrieves an active session for a user by their external (Telegram) user ID.
    /// </summary>
    /// <param name="externalUserId">Telegram user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Active session DTO or null if not found.</returns>
    Task<UserSessionDto?> GetActiveSessionAsync(long externalUserId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a session for a user and specific scenario.
    /// </summary>
    /// <param name="externalUserId">Telegram user identifier.</param>
    /// <param name="scenarioId">Scenario identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Session DTO or null if not found.</returns>
    Task<UserSessionDto?> GetSessionByUserIdAndScenarioAsync(long externalUserId, long scenarioId, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new session or updates an existing one.
    /// </summary>
    /// <param name="externalUserId">Telegram user identifier.</param>
    /// <param name="scenarioId">Scenario identifier.</param>
    /// <param name="stepId">Current step identifier.</param>
    /// <param name="data">JSON data payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated or created session DTO.</returns>
    Task<UserSessionDto> CreateOrUpdateSessionAsync(long externalUserId, long scenarioId, long stepId, string data, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the current step of a session.
    /// </summary>
    /// <param name="sessionId">Session identifier.</param>
    /// <param name="stepId">New step identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated session DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when session is not found.</exception>
    Task<UserSessionDto> UpdateCurrentStepAsync(long sessionId, long stepId, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the data payload of a session.
    /// </summary>
    /// <param name="sessionId">Session identifier.</param>
    /// <param name="data">JSON data payload to store.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated session DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when session is not found.</exception>
    Task<UserSessionDto> UpdateSessionDataAsync(long sessionId, string data, CancellationToken cancellationToken);

    /// <summary>
    /// Marks a session as completed.
    /// </summary>
    /// <param name="sessionId">Session identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CompleteSessionAsync(long sessionId, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a session permanently.
    /// </summary>
    /// <param name="sessionId">Session identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteSessionAsync(long sessionId, CancellationToken cancellationToken);
}