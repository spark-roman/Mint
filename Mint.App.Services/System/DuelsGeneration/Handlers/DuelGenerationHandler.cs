using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;

namespace Mint.App.Services.System.DuelsGeneration.Handlers;

/// <inheritdoc/>
public class DuelGenerationHandler(
    IDuelRepository duelRepository,
    TimeProvider timeProvider) : IDuelGenerationHandler
{
    private readonly IDuelRepository _duelRepository = duelRepository ?? throw new ArgumentNullException(nameof(duelRepository));

    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc/>
    public async Task HandleAsync(IReadOnlyCollection<DuelCreateDto> duelCreateDtos, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(duelCreateDtos);

        foreach (var duelCreateDto in duelCreateDtos)
        {
            await HandleAsync(duelCreateDto, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public Task HandleAsync(DuelCreateDto duelCreateDto, CancellationToken cancellationToken)
    {
        return _duelRepository.CreateDuelAsync(duelCreateDto, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> PublishDuelsAsync(CancellationToken cancellationToken)
    {
        var expiresAt = _timeProvider.GetUtcNow().AddHours(24);

        return _duelRepository.PublishDuelsAsync(expiresAt, cancellationToken);
    }
}
