using Microsoft.EntityFrameworkCore;
using Mint.Bot.Infrastructure;
using Mint.Database;
using Mint.App.Services.Infrastructure.DI;
using Mint.Database.Infrastructure.DI;
using Mint.App.Services.Infrastructure.DI.System.Hangfire;
using Hangfire;
using Mint.App.Services.Infrastructure.DI.System.Jobs;
using Mint.App.Services.System.News.Handlers;
using Mint.App.Services.System.News.Google;
using Mint.App.Services.System.DuelsGeneration.Services;
using Mint.App.Services.System.DuelsGeneration.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});
builder.Logging.AddDebug();

builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Empty connection string");
builder.AddHangfireServices(connectionString);
builder.Services.AddDbContextFactory<MintDbContext>(options => options.UseNpgsql(connectionString, options => options.CommandTimeout(600)));

builder.Services.RegisterAppServices(builder.Configuration);
builder.Services.RegisterDatabaseServices();
builder.Services.AddLogging();
builder.RegisterTgBotServices();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "staging")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.ApplyMigrations();

app.UseHangfireDashboard("/hangfire");

// News generation endpoints
app.MapPost("/api/news/collect", async (ITavilyNewsCollectionHandler collector, CancellationToken ct) =>
{
    var result = await collector.CollectAsync(ct);
    return Results.Ok(new
    {
        result.TotalFetched,
        result.NewSaved,
        result.SkippedDuplicates,
        result.FailedSources,
        result.Errors
    });
});

app.MapPost("/api/news/collect-google", async (IGoogleNewsCollectionHandler collector, CancellationToken ct) =>
{
    var result = await collector.CollectAsync(ct);
    return Results.Ok(new
    {
        result.TotalFetched,
        result.NewSaved,
        result.SkippedDuplicates,
        result.FailedSources,
        result.Errors
    });
});

// Duel generation endpoint
app.MapPost("/api/duels/generate-from-news", async (IDuelGenerationService duelGenService, IDuelGenerationHandler duelHandler, CancellationToken ct) =>
{
    var tomorrow = DateTimeOffset.UtcNow.Date.AddDays(1);
    var expiresAt = new DateTimeOffset(tomorrow.Year, tomorrow.Month, tomorrow.Day, 6, 30, 0, TimeSpan.Zero);
    var duels = await duelGenService.GenerateDuelsWithGigaChatFromNewsAsync(5, expiresAt, ct);

    if (duels.Count == 0)
    {
        return Results.Ok(new { Message = "No duels generated" });
    }

    await duelHandler.HandleAsync(duels, ct);

    return Results.Ok(new { GeneratedDuels = duels.Count });
});

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.ScheduleRecurringJobs();
}

var port = Environment.GetEnvironmentVariable("PORT");
await app.RunAsync($"http://*:{port}");


