using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mint.App.Services.Infrastructure.DI;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.System.DuelsGeneration;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.App.Services.System.DuelsGeneration.Services;
using Mint.App.Services.System.News.Handlers;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.Database.Infrastructure.DI.Design;
using Telegram.Bot.Types;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        var basePath = Directory.GetCurrentDirectory();
        var jsonPath = Path.Combine(basePath, "appsettings.json");
        config.AddJsonFile(jsonPath, optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Empty connection string");

        services.Configure<TelegramOptions>(context.Configuration.GetSection(TelegramOptions.SectionName));
        services.Configure<DeepSeekSettings>(context.Configuration.GetSection("DeepSeekSettings"));
        services.AddDbContextFactory<MintDbContext>(options => options.UseNpgsql(connectionString));

        services.RegisterAppServices(context.Configuration);
        services.RegisterDatabaseServices();
    })
    .Build();

    var duelGenService = host.Services.GetRequiredService<IDuelGenerationService>();

    using var cts = new CancellationTokenSource();

    await duelGenService.GenerateDuelsWithGigaChatAsync(1, 1, cts.Token);

    Console.ReadKey();
