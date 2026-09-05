using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Transactions;
using Mint.Database.Entities.News;
using Mint.Database.Entities.News.RSS;
using Mint.Database.Entities.Prompts.System;
using Mint.Database.Entities.System.Payouts;
using Mint.Database.Entities.System.Settings;
using Mint.Database.Entities.UserInteractive.Bonuses;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Stats;
using Mint.Database.Entities.UserInteractive.UserCategories;
using Mint.Database.Entities.UserInteractive.Votes;
using Mint.Database.Entities.Users;
using Mint.Database.Entities.Users.Sessions;
using Mint.Database.Infrastructure.Data;

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
    /// Duel options
    /// </summary>
    public DbSet<DuelOptionEntity> DuelOptions { get; set; }

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
    /// User stats
    /// </summary>
    public DbSet<UserStatsEntity> UserStats { get; set; }

    /// <summary>
    /// User bonus stats
    /// </summary>
    public DbSet<UserBonusStatsEntity> UserBonusStats { get; set; }

    /// <summary>
    /// Rank configurations
    /// </summary>
    public DbSet<RankConfigEntity> RankConfigs { get; set; }

    /// <summary>
    /// Bonus types
    /// </summary>
    public DbSet<BonusTypeEntity> BonusTypes { get; set; }

    /// <summary>
    /// Step types
    /// </summary>
    public DbSet<StepTypeEntity> StepTypes { get; set; }

    /// <summary>
    /// Scenarios
    /// </summary>
    public DbSet<ScenarioEntity> Scenarios { get; set; }

    /// <summary>
    /// Steps
    /// </summary>
    public DbSet<StepEntity> Steps { get; set; }

    /// <summary>
    /// Buttons
    /// </summary>
    public DbSet<ButtonEntity> Buttons { get; set; }

    /// <summary>
    /// User sessions
    /// </summary>
    public DbSet<UserSessionEntity> UserSessions { get; set; }

    /// <summary>
    /// RSS sources
    /// </summary>
    /// <value></value>
    public DbSet<RssSourceEntity> RssSources { get; set; }

    /// <summary>
    /// News
    /// </summary>
    public DbSet<NewsEntity> News { get; set; }

    /// <summary>
    /// System settings
    /// </summary>
    public DbSet<SystemSettingEntity> SystemSettings { get; set; }

    /// <summary>
    /// Payouts
    /// </summary>
    public DbSet<PayoutEntity> Payouts { get; set; }

    /// <summary>
    /// Constructor with connection param
    /// </summary>
    /// <param name="options">Db context options</param>
    /// <returns></returns>
    public MintDbContext(DbContextOptions<MintDbContext> options) : base(options)
    {
        
    }

    private readonly long _godUserExternalId = -1;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => new { u.ExternalUserId, u.SystemType })
            .IsUnique()
            .HasDatabaseName("IX_users_external_user_id_system_type");

        modelBuilder.Entity<UserEntity>()
            .HasQueryFilter(u => u.ExternalUserId != _godUserExternalId);

        modelBuilder.Entity<VoteEntity>(entity =>
        {
            entity.HasKey(v => v.Id);

            entity.HasIndex(v => new { v.AccountId, v.DuelId })
                .IsUnique()
                .HasDatabaseName("IX_votes_account_duel_unique");

            entity.HasOne(v => v.Account)
                .WithMany(a => a.Votes)
                .HasForeignKey(v => v.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.Duel)
                .WithMany(d => d.Votes)
                .HasForeignKey(v => v.DuelId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<VoteEntity>()
                .HasOne(v => v.ChosenOption)
                .WithMany(o => o.Votes)
                .HasForeignKey(v => v.ChosenOptionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DuelEntity>()
            .HasOne(d => d.Category)
            .WithMany(c => c.Duels)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DuelEntity>()
            .Property(u => u.Status)
            .HasDefaultValue(DuelStatus.Closed);

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

        modelBuilder.Entity<UserStatsEntity>()
            .HasOne(us => us.User)
            .WithOne(u => u.Stats)
            .HasForeignKey<UserStatsEntity>(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserBonusStatsEntity>()
            .HasOne(ubs => ubs.User)
            .WithOne(u => u.BonusStats)
            .HasForeignKey<UserBonusStatsEntity>(ubs => ubs.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RankConfigEntity>()
            .HasIndex(r => r.Code)
            .IsUnique()
            .HasDatabaseName("IX_ranks_config_code");

        modelBuilder.Entity<TransactionEntity>()
            .HasOne(t => t.TransactionType)
            .WithMany()
            .HasForeignKey(t => t.BonusTypeId);

        modelBuilder.Entity<AccountEntity>()
            .Property(a => a.Balance)
            .HasPrecision(12, 2);

        modelBuilder.Entity<TransactionEntity>()
            .Property(t => t.Amount)
            .HasPrecision(12, 2);

        modelBuilder.Entity<VoteEntity>()
            .Property(v => v.BetAmount)
            .HasPrecision(12, 2);

        modelBuilder.Entity<UserBonusStatsEntity>(entity =>
        {
            entity.Property(b => b.TotalStartBonusesClaimed)
                .HasPrecision(12, 2);

            entity.Property(b => b.TotalStreakBonusesClaimed)
                .HasPrecision(12, 2);

            entity.Property(b => b.TotalDailyBonusesClaimed)
                .HasPrecision(12, 2);

            entity.Property(b => b.TotalReferralBonusesClaimed)
                .HasPrecision(12, 2);

            entity.Property(b => b.TotalRankBonusClaimed)
                .HasPrecision(12, 2);
        });

        modelBuilder.Entity<ButtonEntity>(entity =>
        {
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Caption)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(b => b.Action)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(b => b.Type)
                .HasColumnType("smallint")
                .HasDefaultValueSql("1");

            entity.HasOne(b => b.ParentStep)
                .WithMany(s => s.Buttons)
                .HasForeignKey(b => b.ParentStepId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.NextStep)
                .WithMany()
                .HasForeignKey(b => b.NextStepId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<UserSessionEntity>()
            .HasOne(us => us.Scenario)
            .WithMany()
            .HasForeignKey(us => us.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserSessionEntity>()
            .HasOne(us => us.CurrentStep)
            .WithMany()
            .HasForeignKey(us => us.CurrentStepId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RssSourceEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Url)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.CategoryCode)
                .HasMaxLength(50);

            entity.HasIndex(e => e.Url)
                .IsUnique();

            entity.HasIndex(e => e.CategoryCode)
                .HasDatabaseName("idx_rss_sources_category");

            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("idx_rss_sources_active");

            entity.HasIndex(e => e.Priority)
                .HasDatabaseName("idx_rss_sources_priority"); 

            entity.HasIndex(e => e.Language)
                .HasDatabaseName("idx_rss_sources_language");
        });

        modelBuilder.Entity<NewsEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Link)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.Description)
                .HasColumnType("text");

            entity.Property(e => e.Content)
                .HasColumnType("text");

            entity.Property(e => e.Author)
                .HasMaxLength(200);

            entity.Property(e => e.CategoryCode)
                .HasMaxLength(50);

            entity.HasIndex(e => e.Link)
                .IsUnique()
                .HasDatabaseName("idx_news_link_unique");

            entity.HasIndex(e => e.PublishedAt)
                .IsDescending()
                .HasDatabaseName("idx_news_published_at_desc");

            entity.HasIndex(e => e.IsProcessed)
                .HasDatabaseName("idx_news_is_processed");

            entity.HasIndex(e => e.CategoryCode)
                .HasDatabaseName("idx_news_category");

            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("idx_news_created_at");

            entity.HasIndex(e => e.RssSourceId)
                .HasDatabaseName("idx_news_rss_source");

            entity.HasIndex(e => new { e.IsProcessed, e.PublishedAt })
                .HasDatabaseName("idx_news_processed_published");

            entity.HasIndex(e => new { e.CategoryCode, e.IsProcessed })
                .HasDatabaseName("idx_news_category_processed");

            entity.HasOne(e => e.RssSource)
                .WithMany(s => s.News)
                .HasForeignKey(e => e.RssSourceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PayoutEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Amount)
                .IsRequired()
                .HasPrecision(12, 2);

            entity.Property(e => e.Status)
                .IsRequired();

            entity.Property(e => e.ProcessedAt)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.HasIndex(e => e.VoteId)
                .HasDatabaseName("idx_payouts_vote_id");

            entity.HasIndex(e => e.DuelId)
                .HasDatabaseName("idx_payouts_duel_id");

            entity.HasIndex(e => e.AccountId)
                .HasDatabaseName("idx_payouts_account_id");

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("idx_payouts_status");

            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("idx_payouts_created_at");

            entity.HasIndex(e => new { e.DuelId, e.Status })
                .HasDatabaseName("idx_payouts_duel_status");

            entity.HasIndex(e => new { e.AccountId, e.Status })
                .HasDatabaseName("idx_payouts_account_status");

            entity.HasOne(e => e.Vote)
                .WithMany()
                .HasForeignKey(e => e.VoteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Duel)
                .WithMany()
                .HasForeignKey(e => e.DuelId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Transaction)
                .WithMany()
                .HasForeignKey(e => e.TransactionId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        modelBuilder.InitData();
    }
}