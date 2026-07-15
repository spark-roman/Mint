using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Bot.Commands;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <inheritdoc/>
public class ButtonHandlerFactory(IServiceProvider serviceProvider) : IButtonHandlerFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <inheritdoc/>
    public IButtonHandler? Create(TgCommandType commandType)
    {
        return _serviceProvider.GetKeyedService<IButtonHandler>(commandType);
    }
}
