using Microsoft.EntityFrameworkCore;
using Mint.Bot.Infrastructure;
using Mint.Database;
using Mint.App.Services.Infrastructure.DI;
using Mint.Database.Infrastructure.DI;

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

builder.Services.AddDbContextFactory<MintDbContext>(options => 
    options.UseNpgsql(connectionString, options => options.CommandTimeout(600)));
builder.Services.RegisterAppServices();
builder.Services.RegisterDatabaseServices();
builder.RegisterTgBotServices();
builder.Services.AddLogging();

var app = builder.Build();

await app.ApplyMigrations();

var port = Environment.GetEnvironmentVariable("PORT");
await app.RunAsync($"http://*:{port}");


