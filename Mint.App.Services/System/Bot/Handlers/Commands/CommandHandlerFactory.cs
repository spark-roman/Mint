using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Bot.Commands;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc/>
public class CommandHandlerFactory(IServiceProvider serviceProvider) : ICommandHandlerFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <inheritdoc/>
    public ICommandHandler? Create(TgCommandType commandType)
    {
        return _serviceProvider.GetKeyedService<ICommandHandler>(commandType);
    }
}
