using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.Infrastructure.DI.System.Duels;

namespace Mint.UnitTests.AppServices.System.WinCalculation.WinCalculationRules.Fixtures;

/// <summary>
/// Fixture for OpinionMatchRule tests with EF Core In-Memory and DI.
/// </summary>
public sealed class OpinionMatchRuleFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpinionMatchRuleFixture"/> class.
    /// </summary>
    public OpinionMatchRuleFixture()
    {
        var services = new ServiceCollection();
        services.RegisterCalculationRulesServices();

        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Gets the service provider.
    /// </summary>
    public IServiceProvider ServiceProvider => _serviceProvider;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;

        _serviceProvider?.Dispose();
        _disposed = true;
    }
}
