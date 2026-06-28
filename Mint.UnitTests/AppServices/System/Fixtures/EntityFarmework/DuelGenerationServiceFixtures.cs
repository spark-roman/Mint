using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.Infrastructure.DI.System;
using Mint.App.Services.System.DuelsGeneration;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.UnitTests.AppServices.System.Fixtures.Seeding;
using Mint.App.Services.Infrastructure.DI;

namespace Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;

/// <summary>
/// Fixture for <see cref="DuelGenerationService"/> tests using EF InMemory and DI registrations.
/// </summary>
public sealed class DuelGenerationServiceFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly HttpMessageHandler _httpClientHandler;
    private readonly string _databaseName;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelGenerationServiceFixture"/> class.
    /// </summary>
    public DuelGenerationServiceFixture()
    {
        _databaseName = "DuelGenerationTest" + Guid.NewGuid();

        var services = new ServiceCollection();

        services.RegisterDatabaseServices();
        services.RegisterAppServices();

        services.AddEntityFrameworkInMemoryDatabase();
        services.AddDbContextFactory<MintDbContext>(options => options.UseInMemoryDatabase(_databaseName));

        _httpClientHandler = new MockHttpMessageHandler();
        services.RegisterDeepSeekHttpClient(new HttpClient(_httpClientHandler));

        _serviceProvider = services.BuildServiceProvider();

        SeedData();
    }

    /// <summary>
    /// Seeds the database with test data using DuelsSeeder.
    /// </summary>
    private void SeedData()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = dbContextFactory.CreateDbContextAsync().GetAwaiter().GetResult();
        DuelsSeeder.Seed(context);
    }

    /// <summary>
    /// Creates a service scope for resolving services.
    /// </summary>
    /// <returns>Service scope.</returns>
    public IServiceScope CreateScope()
    {
        return _serviceProvider.CreateScope();
    }

    /// <summary>
    /// Sets the HTTP response for the DeepSeek API mock.
    /// </summary>
    /// <param name="jsonContent">JSON content string.</param>
    public void SetHttpResponse(string jsonContent)
    {
        if (_httpClientHandler is MockHttpMessageHandler handler)
        {
            handler.SetResponse(jsonContent);
        }
    }

    /// <summary>
    /// Sets the HTTP response to simulate an API error.
    /// </summary>
    /// <param name="statusCode">HTTP status code.</param>
    /// <param name="errorMessage">Error message.</param>
    public void SetHttpErrorResponse(HttpStatusCode statusCode, string errorMessage)
    {
        if (_httpClientHandler is MockHttpMessageHandler handler)
        {
            handler.SetError(statusCode, errorMessage);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;

        _httpClientHandler?.Dispose();
        _serviceProvider?.Dispose();
        _disposed = true;
    }

    /// <summary>
    /// Mock HTTP message handler for DeepSeek API.
    /// </summary>
    private sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage _response;

        public MockHttpMessageHandler()
        {
            _response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"choices\":[{\"message\":{\"content\":\"[]\"}}]}",
                    Encoding.UTF8,
                    "application/json")
            };
        }

        public void SetResponse(string jsonContent)
        {
            _response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };
        }

        public void SetError(HttpStatusCode statusCode, string errorMessage)
        {
            _response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(errorMessage, Encoding.UTF8, "application/json")
            };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }

        protected override void Dispose(bool disposing)
        {
            _response.Dispose();
            base.Dispose(disposing);
        }
    }
}

