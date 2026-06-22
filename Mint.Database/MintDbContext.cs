using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mint.Database.Entities.Accounts;
using Mint.Database.Entities.Transactions;
using Mint.Database.Entities.Users;

namespace Mint.Database;

/// <summary>
/// Database context
/// </summary>
public class MintDbContext : DbContext
{
    /// <summary>
    /// Users
    /// </summary>
    public DbSet<UserEntity> Users { get; set; }

    /// <summary>
    /// Accounts
    /// </summary>
    public DbSet<AccountEntity> Accounts { get; set; }

    /// <summary>
    /// Transactions
    /// </summary>
    public DbSet<TransactionEntity> Transactions { get; set; }

    /// <summary>
    /// Constructor with connection param
    /// </summary>
    /// <param name="options">Db context options</param>
    /// <returns></returns>
    public MintDbContext(DbContextOptions<MintDbContext> options) : base(options)
    {
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseNpgsql("YourConnectionString")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
        }
    }
}
