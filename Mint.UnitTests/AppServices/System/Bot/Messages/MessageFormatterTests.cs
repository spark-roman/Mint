using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Profiles.Dto;

namespace Mint.UnitTests.AppServices.System.Bot.Messages;

/// <summary>
/// Tests for <see cref="MessageFormatter"/> leaderboard formatting.
/// </summary>
public class MessageFormatterTests
{
    private readonly MessageFormatter _formatter = new(TimeProvider.System);

    /// <summary>
    /// Verifies that FormatLeaderboardAsync escapes special characters in display names.
    /// </summary>
    [Fact]
    public async Task FormatLeaderboardAsync_DisplayNameWithSpecialChars_EscapesThem()
    {
        // Arrange
        var template = "{{leaderboard_entries}}";
        var leaderboard = new LeaderboardResultDto
        {
            Entries =
            [
                new LeaderboardEntryDto
                {
                    Rank = 1,
                    ExternalUserId = 1001,
                    DisplayName = "user_name.2026",
                    RankName = "Эксперт",
                    RankPoints = 1500m
                }
            ],
            TotalUsers = 1
        };

        // Act
        var result = await _formatter.FormatLeaderboardAsync(template, leaderboard, CancellationToken.None);

        // Assert
        Assert.Contains("user\\_name\\.2026", result);
        Assert.DoesNotContain("user_name.2026", result);
    }

    /// <summary>
    /// Verifies that FormatLeaderboardAsync escapes special characters in rank names.
    /// </summary>
    [Fact]
    public async Task FormatLeaderboardAsync_RankNameWithSpecialChars_EscapesThem()
    {
        // Arrange
        var template = "{{leaderboard_entries}}";
        var leaderboard = new LeaderboardResultDto
        {
            Entries =
            [
                new LeaderboardEntryDto
                {
                    Rank = 1,
                    ExternalUserId = 1001,
                    DisplayName = "alice",
                    RankName = "Мастер (топ)",
                    RankPoints = 1500m
                }
            ],
            TotalUsers = 1
        };

        // Act
        var result = await _formatter.FormatLeaderboardAsync(template, leaderboard, CancellationToken.None);

        // Assert
        Assert.Contains("Мастер \\(топ\\)", result);
        Assert.DoesNotContain("Мастер (топ)", result);
    }

    /// <summary>
    /// Verifies that FormatLeaderboardAsync does not escape names without special characters.
    /// </summary>
    [Fact]
    public async Task FormatLeaderboardAsync_NamesWithoutSpecialChars_ReturnsUnescaped()
    {
        // Arrange
        var template = "{{leaderboard_entries}}";
        var leaderboard = new LeaderboardResultDto
        {
            Entries =
            [
                new LeaderboardEntryDto
                {
                    Rank = 1,
                    ExternalUserId = 1001,
                    DisplayName = "alice",
                    RankName = "Эксперт",
                    RankPoints = 1500m
                }
            ],
            TotalUsers = 1
        };

        // Act
        var result = await _formatter.FormatLeaderboardAsync(template, leaderboard, CancellationToken.None);

        // Assert
        Assert.Contains("alice", result);
        Assert.Contains("Эксперт", result);
    }

    /// <summary>
    /// Verifies that FormatLeaderboardAsync keeps the entry structure (medal, rank, points) after escaping.
    /// </summary>
    [Fact]
    public async Task FormatLeaderboardAsync_EscapedEntry_KeepsStructure()
    {
        // Arrange
        var template = "{{leaderboard_entries}}";
        var leaderboard = new LeaderboardResultDto
        {
            Entries =
            [
                new LeaderboardEntryDto
                {
                    Rank = 1,
                    ExternalUserId = 1001,
                    DisplayName = "u.ser",
                    RankName = "Эксп.ерт",
                    RankPoints = 1500m
                }
            ],
            TotalUsers = 1
        };

        // Act
        var result = await _formatter.FormatLeaderboardAsync(template, leaderboard, CancellationToken.None);

        // Assert
        Assert.Contains("🥇 **1.** u\\.ser — Эксп\\.ерт • 1,500 RP", result);
    }

    /// <summary>
    /// Verifies that FormatLeaderboardAsync escapes each entry independently.
    /// </summary>
    [Fact]
    public async Task FormatLeaderboardAsync_MultipleEntries_EscapesEach()
    {
        // Arrange
        var template = "{{leaderboard_entries}}";
        var leaderboard = new LeaderboardResultDto
        {
            Entries =
            [
                new LeaderboardEntryDto
                {
                    Rank = 1,
                    ExternalUserId = 1001,
                    DisplayName = "a*b",
                    RankName = "Эксперт",
                    RankPoints = 1500m
                },
                new LeaderboardEntryDto
                {
                    Rank = 2,
                    ExternalUserId = 1002,
                    DisplayName = "bob",
                    RankName = "Но!вичок",
                    RankPoints = 300m
                }
            ],
            TotalUsers = 2
        };

        // Act
        var result = await _formatter.FormatLeaderboardAsync(template, leaderboard, CancellationToken.None);

        // Assert
        Assert.Contains("🥇 **1.** a\\*b — Эксперт • 1,500 RP", result);
        Assert.Contains("🥈 **2.** bob — Но\\!вичок • 300 RP", result);
    }

    /// <summary>
    /// Verifies that FormatLeaderboardAsync returns the empty state message when there are no entries.
    /// </summary>
    [Fact]
    public async Task FormatLeaderboardAsync_NoEntries_ReturnsEmptyStateMessage()
    {
        // Arrange
        var template = "{{leaderboard_entries}}";
        var leaderboard = new LeaderboardResultDto
        {
            Entries = [],
            TotalUsers = 0
        };

        // Act
        var result = await _formatter.FormatLeaderboardAsync(template, leaderboard, CancellationToken.None);

        // Assert
        Assert.Equal("Пока нет участников. Станьте первым! 🚀", result);
    }

    /// <summary>
    /// Verifies that FormatLeaderboardAsync replaces the user rank info placeholder.
    /// </summary>
    [Fact]
    public async Task FormatLeaderboardAsync_WithUserRank_ReplacesUserRankInfo()
    {
        // Arrange
        var template = "{{user_rank_info}}";
        var leaderboard = new LeaderboardResultDto
        {
            Entries = [],
            TotalUsers = 1,
            UserRank = 4,
            UserEntry = new LeaderboardEntryDto
            {
                Rank = 4,
                ExternalUserId = 1004,
                DisplayName = "diana_p",
                RankName = "Эксперт",
                RankPoints = 300m
            }
        };

        // Act
        var result = await _formatter.FormatLeaderboardAsync(template, leaderboard, CancellationToken.None);

        // Assert
        Assert.Equal("👤 **Ваше место в рейтинге:** #4 (300 RP)", result);
    }

    /// <summary>
    /// Verifies that FormatLeaderboardAsync returns the no-participation message when the user has no rank.
    /// </summary>
    [Fact]
    public async Task FormatLeaderboardAsync_WithoutUserRank_ReturnsNoParticipationMessage()
    {
        // Arrange
        var template = "{{user_rank_info}}";
        var leaderboard = new LeaderboardResultDto
        {
            Entries = [],
            TotalUsers = 0
        };

        // Act
        var result = await _formatter.FormatLeaderboardAsync(template, leaderboard, CancellationToken.None);

        // Assert
        Assert.Equal("👤 **Вы ещё не участвовали в дуэлях**", result);
    }

    /// <summary>
    /// Verifies that FormatLeaderboardAsync replaces the total users placeholder.
    /// </summary>
    [Fact]
    public async Task FormatLeaderboardAsync_ReplacesTotalUsers()
    {
        // Arrange
        var template = "{{total_users}}";
        var leaderboard = new LeaderboardResultDto
        {
            Entries = [],
            TotalUsers = 42
        };

        // Act
        var result = await _formatter.FormatLeaderboardAsync(template, leaderboard, CancellationToken.None);

        // Assert
        Assert.Equal("42", result);
    }

    /// <summary>
    /// Verifies that FormatLeaderboardAsync throws ArgumentNullException when the leaderboard result is null.
    /// </summary>
    [Fact]
    public async Task FormatLeaderboardAsync_NullLeaderboard_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => _formatter.FormatLeaderboardAsync("template", null!, CancellationToken.None));
    }
}