using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Handlers;

/// <summary>
/// Handles generation of duels from AI.
/// </summary>
public interface IDuelGenerationHandler
{
    /// <summary>
    /// Handle generation of duels from AI.
    /// </summary>
    /// <param name="duelCreateDtos">Dtos of duel to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task HandleAsync(IReadOnlyCollection<DuelCreateDto> duelCreateDtos,CancellationToken cancellationToken);

    /// <summary>
    /// Handle generation of duels from AI.
    /// </summary>
    /// <param name="duelCreateDto">Dto of duel to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    Task HandleAsync(DuelCreateDto duelCreateDto,CancellationToken cancellationToken);
}
