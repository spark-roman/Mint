using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mint.Database.Entities.Users;

/// <summary>
/// User entity
/// </summary>
[Table("users")]
public class UserEntity
{
    /// <summary>
    /// Internal user id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// Vendor user id
    /// </summary>
    [Required]
    [Column("external_user_id")]
    public long ExternalUserId { get; set; }

    /// <summary>
    /// Auth system type
    /// </summary>
    [Column("system_type")]
    public byte SystemType { get; set; }

    /// <summary>
    /// First name
    /// </summary>
    [StringLength(500)]
    [Column("first_name")]
    public required string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    [StringLength(500)]
    [Column("last_name")]
    public required string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User name
    /// </summary>
    [StringLength(500)]
    [Column("user_name")]
    public required string UserName { get; set; } = string.Empty;

    /// <summary>
    /// User creation date
    /// </summary>
    [Column("created_at")]
    public DateTimeOffset CreatedAt  { get; set; }

    /// <summary>
    /// Last auth date
    /// </summary>
    [Column("last_auth_date")]
    public DateTimeOffset? LastAuthDate { get; set; }

    /// <summary>
    /// User status
    /// </summary>
    [Column("status")]
    public byte Status { get; set; }
}