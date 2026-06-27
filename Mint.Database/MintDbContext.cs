using Microsoft.EntityFrameworkCore;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Transactions;
using Mint.Database.Entities.System;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.UserCategories;
using Mint.Database.Entities.UserInteractive.Votes;
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
    /// Duels
    /// </summary>
    public DbSet<DuelEntity> Duels { get; set; }

    /// <summary>
    /// Votes
    /// </summary>
    public DbSet<VoteEntity> Votes { get; set; }

    /// <summary>
    /// AI prompts and settings
    /// </summary>
    public DbSet<AiPromptEntity> AiPrompts { get; set; }

    /// <summary>
    /// User categories
    /// </summary>
    public DbSet<CategoryEntity> UserCategories { get; set; }

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

        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => new { u.ExternalUserId, u.SystemType })
            .IsUnique()
            .HasDatabaseName("IX_users_external_user_id_system_type");

        modelBuilder.Entity<VoteEntity>()
            .HasKey(v => new { v.AccountId, v.DuelId });

        modelBuilder.Entity<VoteEntity>()
            .HasIndex(v => new { v.DuelId, v.AccountId })
            .IsUnique()
            .HasDatabaseName("IX_votes_duel_id_account_id");

        modelBuilder.Entity<VoteEntity>()
            .HasOne(v => v.Duel)
            .WithMany(d => d.Votes)
            .HasForeignKey(v => v.DuelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VoteEntity>()
            .HasOne(v => v.Account)
            .WithMany()
            .HasForeignKey(v => v.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VoteEntity>()
            .HasOne(v => v.ChosenOption)
            .WithMany(o => o.Votes)
            .HasForeignKey(v => v.ChosenOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DuelEntity>()
            .HasOne(d => d.Category)
            .WithMany()
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DuelOptionEntity>()
            .HasOne(o => o.Duel)
            .WithMany(d => d.Options)
            .HasForeignKey(o => o.DuelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AiPromptEntity>()
            .HasMany(p => p.Categories)
            .WithOne(c => c.AiPrompt)
            .HasForeignKey(c => c.AiPromptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}