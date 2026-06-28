namespace Mint.App.Services.UserInteractive.Users.Dto;

/// <summary>
/// This dto represents a Telegram user or bot.
/// </summary>
public record ExternalUserDto
{
    /// <summary>
    /// Unique identifier for this user or bot.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// If this user is a bot.
    /// </summary>
    public bool IsBot { get; init; }

    /// <summary>
    /// User's or bot’s first name.
    /// </summary>
    public string FirstName { get; init; } = default!;

    /// <summary>
    /// Optional. User's or bot’s last name.
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// Optional. User's or bot’s username.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Optional. <see langword="true"/>, if this user is a Telegram Premium user.
    /// </summary>
    public bool? IsPremium { get; init; }

    /// <summary>
    /// Optional. <see langword="true"/>, if this user added the bot to the attachment menu.
    /// </summary>
    public bool? AddedToAttachmentMenu { get; init; }

    /// <summary>
    /// Optional. <see langword="true"/>, if the bot can be invited to groups.
    /// </summary>
    public bool? CanJoinGroups { get; init; }

    /// <summary>
    /// Optional. <see langword="true"/>, if privacy mode is disabled for the bot.
    /// </summary>
    public bool? CanReadAllGroupMessages { get; init; }

    /// <summary>
    /// Optional. <see langword="true"/>, if the bot supports inline queries.
    /// </summary>
    public bool? SupportsInlineQueries { get; init; }
}
