using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mint.Database.Entities.Users;

/// <summary>
/// User entity
/// </summary>
public class UserEntity
{
    /// <summary>
    /// Internal user id
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// Vendor user id
    /// </summary>
    [Required]
    public long ExternalUserId { get; set; }

    /// <summary>
    /// Auth system type
    /// </summary>
    public byte SystemType { get; set; }

    /// <summary>
    /// First name
    /// </summary>
    [Column(TypeName = "VARCHAR")]
    [StringLength(500)]
    public required string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    [Column(TypeName = "VARCHAR")]
    [StringLength(500)]
    public required string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User name
    /// </summary>
    [Column(TypeName = "VARCHAR")]
    [StringLength(500)]
    public required string UserName { get; set; } = string.Empty;

    /// <summary>
    /// User creation date
    /// </summary>
    public DateTimeOffset CreatedAt  { get; set; }

    /// <summary>
    /// Last auth date
    /// </summary>
    public DateTimeOffset? LastAuthDate { get; set; }

    /// <summary>
    /// User status
    /// </summary>
    public byte Status { get; set; }
}