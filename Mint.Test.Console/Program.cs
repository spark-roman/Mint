using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mint.App.Services.Infrastructure.DI;
using Mint.App.Services.System.DuelsGeneration;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.Database.Infrastructure.DI.Design;

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

        services.Configure<DeepSeekSettings>(context.Configuration.GetSection("DeepSeekSettings"));
        services.AddDbContextFactory<MintDbContext>(options => options.UseNpgsql(connectionString));
        //services.AddMintDesignTimeDbContext();

        services.RegisterAppServices();
        services.RegisterDatabaseServices();
    })
    .Build();

    var duelGenerationService = host.Services.GetRequiredService<IDuelGenerationService>();
    var result = await duelGenerationService.GenerateDuelsForAllActiveCategoriesAsync(3, CancellationToken.None);
